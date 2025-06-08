using System;
using HexKit3D;
using UnityEngine;
using MEC;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.InputSystem.Android;
using Unity.VisualScripting;

public abstract class NPC : Entity
{

    [SerializeField] protected Animator anim;

    //Assigns
    [Header("Assigns")]
    [SerializeField] Island m_assignedIsland;
    public Island assignedIsland
    {
        get => m_assignedIsland;
        set
        {
            if(m_assignedIsland != value)
            {
                var prev = m_assignedIsland;
                m_assignedIsland = value;
                onAssignedIslandChange?.Invoke(prev, m_assignedIsland);
            }
        }
    }
    public Action<Island, Island> onAssignedIslandChange;
    public IResidental home { get; private set; }
    IWorkplace m_workplace;
    public IWorkplace workplace
    {
        get => m_workplace;
        set
        {
            if(m_workplace != value)
            {
                if (m_workplace != null) m_workplace.workers.Remove(this);
                m_workplace = value;
                if (m_workplace != null) m_workplace.workers.Add(this);
            }
        }
    }


    [Header("Navigation")]
    public float moveSpeedMultiplier = 1.0f;
    public Cubic position => assignedIsland.tilemap.PosToCubic(transform.position);


    [Header("Equipment")]
    [SerializeField] protected Transform sheatheAnchor;
    [SerializeField] protected Transform heldAnchor;

    //rotation
    float rotateRate => GameManager.Instance.npcRotateRate;
    protected float targetRotation = 0.0f;

    //height
    float heightRate => GameManager.Instance.npcHeightRate;

    //navigation
    public bool isNavigating { get; private set; } = false;
    float moveSpeed => GameManager.Instance.baseNPCMoveSpeed * moveSpeedMultiplier;
    float stopDistance => GameManager.Instance.npcNavigationStopDistance;
    (CoroutineHandle, Action) navigating;

    //energy & work
    public float energy = 10.0f;
    public Equipment equipment { get; private set; }

    //Levels
    public int level { get; private set; } = 1;
    private void OnDrawGizmos()
    {
        if (navigatingPath != null)
        {
            for (int i = 1; i < navigatingPath.route.Count; i++)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(navigatingPath.route[i - 1].transform.position, navigatingPath.route[i].transform.position);
            }
        }
    }
    protected virtual void Update()
    {
        RotationUpdate();
        GroundCheckUpdate();
    }
    void RotationUpdate()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, targetRotation, 0), rotateRate * Time.deltaTime);
    }
    void GroundCheckUpdate()
    {
        if(assignedIsland.tilemap.TryGetTile(position, out MapTile tile))
        {
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, tile.height, heightRate * Time.deltaTime), transform.position.z);
        }
    }
    HexTilemapPath<MapTile> navigatingPath = null;
    public void Navigate(HexTilemapPath<MapTile> path, Action onFinish = null)
    {
        if (isNavigating) StopNavigating();
        if(path == null)
        {
            onFinish?.Invoke(); return;
        }
        isNavigating = true;
        navigatingPath = path;
        navigating = (Timing.RunCoroutine(Navigating(path, onFinish)), onFinish);
    }
    public void StopNavigating()
    {
        if (!isNavigating) return;
        Timing.KillCoroutines(navigating.Item1);
        navigatingPath = null;
        isNavigating = false;
        navigating.Item2?.Invoke();
    }
    Vector3 currentPos => new Vector3(transform.position.x, 0.0f, transform.position.z);
    IEnumerator<float> Navigating(HexTilemapPath<MapTile> path, Action onFinish)
    {
        foreach(var next in path.route)
        {
            Vector3 targetPos = new Vector3(next.transform.position.x, 0.0f, next.transform.position.z);
            while (Vector3.Distance(currentPos, targetPos) > stopDistance)
            {
                if (!next.isWalkable)
                {
                    isNavigating = false;
                    onFinish?.Invoke();
                    yield break;
                }
                targetRotation = Mathf.Atan2(targetPos.x - currentPos.x, targetPos.z - currentPos.z) * Mathf.Rad2Deg;
                transform.Translate(Vector3.ClampMagnitude((targetPos - currentPos).normalized * moveSpeed * Time.deltaTime, Vector3.Distance(targetPos, currentPos)), Space.World);
                yield return Timing.WaitForOneFrame;
            }
        }
        StopNavigating();
    }
    bool earlyRest = false;
    void WakeUp()
    {
        energy = home.energyGiven;
        var foodList = EmpireManager.Instance.SearchAll(item => item is FoodData);
        if (foodList.Count <= 0) energy = 10.0f;
        else
        {
            foodList.Sort((a, b) => (b as FoodData).selectionPriority.CompareTo((b as FoodData).selectionPriority));
            EmpireManager.Instance.RemoveItem(foodList[0], 1);
            (foodList[0] as FoodData).OnConsume(this);
        }
    }
    public virtual void LoseEnergy(float amount)
    {
        energy -= amount;
    }
    public void SetEquipment(EquipmentData equipment)
    {
        this.equipment = equipment.Create();
        Sheathe();
    }
    public void Sheathe()
    {
        if (equipment == null) return;
        equipment.transform.SetParent(sheatheAnchor);
        equipment.transform.localScale = Vector3.one;
        equipment.transform.localPosition = Vector3.zero;
        equipment.transform.localRotation = Quaternion.identity;
    }
    public void UnSheathe()
    {
        if (equipment == null) return;
        equipment.transform.SetParent(heldAnchor);
        equipment.transform.localScale = Vector3.one;
        equipment.transform.localPosition = Vector3.zero;
        equipment.transform.localRotation = Quaternion.identity;
    }
    protected abstract List<IWorkplace> GetAvailableJobs();
    protected class NPC_FSMVals : FSMVals
    {

    }
    protected abstract class NPC_TopLayer<T> : TopLayer<T> where T : NPC
    {
        protected NPC_TopLayer(T origin, NPC_FSMVals values) : base(origin, values)
        {
            AddState("Homeless", new NPC_Homeless<T>(origin, this));
            AddState("Resting", new NPC_Resting<T>(origin, this));
            AddState("WorkLayer", GetState_WorkLayer());
        }
        public override void OnStateEnter()
        {
            if(origin.home == null)
            {
                currentState = states["Homeless"];
            }
            else if (GameManager.Instance.isWorkTime && origin.energy > 0)
            {
                currentState = states["WorkLayer"];
            }
            else
            {
                currentState = states["Resting"];
            }
        }
        protected abstract NPC_WorkLayer<T> GetState_WorkLayer();

        //Utility States
        protected class NPC_Navigate<T> : State<T> where T : NPC
        {
            readonly Func<Cubic> destinationGetter;
            readonly Action onArrival;
            readonly Func<int> toleranceGetter;
            public NPC_Navigate(T origin, Layer<T> parent, Func<Cubic> destinationGetter, Func<int> toleranceGetter, Action onArrival) : base(origin, parent)
            {
                this.destinationGetter = destinationGetter;
                this.toleranceGetter = toleranceGetter;
                this.onArrival = onArrival;
            }
            const float pathSearchRate = 5.0f;
            float counter = 0.0f;
            public override void OnStateEnter()
            {
                base.OnStateEnter();
                origin.Navigate(origin.assignedIsland.tilemap.FindPath<MapTile>(origin.position, destinationGetter.Invoke()));
                counter = 0.0f;
            }
            public override void OnStateUpdate()
            {
                base.OnStateUpdate();
                if (!origin.isNavigating)
                {
                    if (Cubic.Distance(origin.position, destinationGetter.Invoke()) <= toleranceGetter.Invoke())
                    {
                        onArrival?.Invoke(); return;
                    }
                    else
                    {
                        counter += Time.deltaTime;
                        if (counter >= pathSearchRate)
                        {
                            origin.Navigate(origin.assignedIsland.tilemap.FindPath<MapTile>(origin.position, destinationGetter.Invoke()));
                            counter = 0.0f;
                        }
                    }
                }
            }
            public override void OnStateExit()
            {
                base.OnStateExit();
                if (origin.isNavigating) origin.StopNavigating();
            }
        }


        //Core States
        class NPC_Homeless<T> : State<T> where T : NPC
        {
            public NPC_Homeless(T origin, Layer<T> parent) : base(origin, parent)
            {

            }
            const float checkRate = 1.0f;
            public override void OnStateEnter()
            {
                base.OnStateEnter();
                if (origin.home != null)
                {
                    parentLayer.ChangeState("Resting");
                }
                counter = 0.0f;
                CheckForHome();
            }
            float counter = 0.0f;
            public override void OnStateUpdate()
            {
                base.OnStateUpdate();
                counter += Time.deltaTime;
                if(counter > checkRate)
                {
                    counter = 0.0f;
                    CheckForHome();
                }
                if (origin.home != null)
                {
                    parentLayer.ChangeState("Resting"); return;
                }
            }
            void CheckForHome()
            {
                if (origin.home != null) return;
                foreach(var i in origin.assignedIsland.SearchElements(element => element is IResidental))
                {
                    IResidental residental = i as IResidental;
                    if(residental.residents.Count < residental.maxResidents)
                    {
                        origin.home = residental;
                        residental.residents.Add(origin);
                    }
                }
            }
        }
        protected abstract class NPC_WorkLayer<T> : Layer<T> where T : NPC
        {
            protected NPC_WorkLayer(T origin, Layer<T> parent) : base(origin, parent)
            {
                defaultState = new LookForWork<T>(origin, this);
                AddState("LookForWork", defaultState);
                AddState("GoToWork", new NPC_Navigate<T>(origin, this,
                    () => origin.workplace.self.position,
                    () => origin.workplace.self.canPass ? 0 : 1,
                    () => ChangeState("Working")));
                AddState("Working", GetState_Working());
            }
            protected abstract NPC_Working<T> GetState_Working();
            public override void OnStateUpdate()
            {
                if(origin.home == null)
                {
                    parentLayer.ChangeState("Homeless"); return;
                }
                if(origin.energy <= 0.0f || !GameManager.Instance.isWorkTime)
                {
                    parentLayer.ChangeState("Resting"); return;
                }
                base.OnStateUpdate();
            }
            protected class NPC_SearchForTools<T, ToolT> : State<T> where T : NPC where ToolT : EquipmentData
            {
                public NPC_SearchForTools(T origin, Layer<T> parent) : base(origin, parent)
                {

                }
                const float searchRate = 1.0f;
                float counter = 0.0f;
                public override void OnStateEnter()
                {
                    base.OnStateEnter();
                    if (origin.equipment != null)
                    {
                        parentLayer.ChangeState("LookForWork"); return;
                    }
                    counter = 0.0f;
                    Search();
                }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();
                    counter += Time.deltaTime;
                    if(counter >= searchRate)
                    {
                        counter = 0.0f;
                        Search();
                    }
                    if(origin.equipment != null)
                    {
                        parentLayer.ChangeState("LookForWork"); return;
                    }
                }
                readonly List<ToolT> searchToolsList = new();
                protected virtual List<ToolT> SearchTools()
                {
                    searchToolsList.Clear();
                    foreach (var i in EmpireManager.Instance.SearchAll(item => item is ToolT)) searchToolsList.Add(i as ToolT);
                    searchToolsList.Sort((a, b) => b.tier.CompareTo(a.tier));
                    return searchToolsList;
                }
                void Search()
                {
                    var list = SearchTools();
                    if(list.Count > 0)
                    {
                        EmpireManager.Instance.RemoveItem(list[0], 1);
                        origin.SetEquipment(list[0]);
                    }
                }
            }
            class LookForWork<T> : State<T> where T : NPC
            {
                public LookForWork(T origin, Layer<T> parent) : base(origin, parent)
                {

                }
                const float searchRate = 1.0f;
                float counter = 0.0f;
                public override void OnStateEnter()
                {
                    base.OnStateEnter();
                    if(origin.workplace != null)
                    {
                        parentLayer.ChangeState("GoToWork"); return;
                    }
                    counter = 0.0f;
                    Search();
                }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();
                    counter += Time.deltaTime;
                    if(counter >= searchRate)
                    {
                        counter = 0.0f;
                        Search();
                    }
                    if(origin.workplace != null)
                    {
                        parentLayer.ChangeState("GoToWork"); return;
                    }
                }
                void Search()
                {
                    var list = origin.GetAvailableJobs();
                    if(list.Count > 0)
                    {
                        origin.workplace = list[0];
                    }
                }
            }
            protected abstract class NPC_Working<T> : Layer<T>
            {
                protected NPC_Working(T origin, Layer<T> parent) : base(origin, parent)
                {

                }
            }
        }
        class NPC_Resting<T> : Layer<T> where T : NPC
        {
            public NPC_Resting(T origin, Layer<T> parent) : base(origin, parent)
            {
                defaultState = new NPC_Navigate<T>(origin, this,
                    () => origin.home.self.position,
                    () => origin.home.self.canPass ? 0 : 1,
                    () => ChangeState("Resting"));
                AddState("GoHome", defaultState);
                AddState("Resting", new Resting(origin, this));
            }
            public override void OnStateEnter()
            {
                base.OnStateEnter();
                if (GameManager.Instance.isWorkTime) origin.earlyRest = true;
            }
            public override void OnStateUpdate()
            {
                if(origin.home == null)
                {
                    parentLayer.ChangeState("Homeless"); return;
                }
                base.OnStateUpdate();
                if (GameManager.Instance.isWorkTime)
                {
                    if (!origin.earlyRest)
                    {
                        origin.WakeUp();
                        parentLayer.ChangeState("WorkLayer"); return;
                    }
                }
                else origin.earlyRest = false;
            }
            class Resting : State<T>
            {
                public Resting(T origin, Layer<T> parent) : base(origin, parent)
                {

                }
            }
        }
    }
}