using System;
using HexKit3D;
using UnityEngine;
using MEC;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.InputSystem.Android;
using Unity.VisualScripting;
using UnityEditor.Rendering;

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

    [Header("Multipliers")]
    public float energyConsuptionMultiplier = 1.0f;
    public float durabilityConsuptionMultiplier = 1.0f;
    public float workAmountMultiplier = 1.0f;

    //rotation
    float rotateRate => GameManager.Instance.npcRotateRate;
    protected float targetRotation = 0.0f;

    //height
    float heightRate => GameManager.Instance.npcHeightRate;

    //navigation

    readonly int isNavigatingID = Animator.StringToHash("IsNavigating");
    bool m_isNavigating = false;
    public bool isNavigating
    {
        get => m_isNavigating;
        set
        {
            if(m_isNavigating != value)
            {
                m_isNavigating = value;
                anim.SetBool(isNavigatingID, value);
            }
        }
    }
    float moveSpeed => GameManager.Instance.baseNPCMoveSpeed * moveSpeedMultiplier;
    float stopDistance => GameManager.Instance.npcNavigationStopDistance;
    CoroutineHandle navigating;

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
    public void Navigate(HexTilemapPath<MapTile> path, int stopTileDistance = 0)
    {
        if (isNavigating) StopNavigating();
        isNavigating = true;
        navigatingPath = path;
        navigating = Timing.RunCoroutine(Navigating(path, stopTileDistance));
    }
    public void StopNavigating()
    {
        if (!isNavigating) return;
        Timing.KillCoroutines(navigating);
        navigatingPath = null;
        isNavigating = false;
    }
    public void LookAt(Vector3 targetPos)
    {
        targetRotation = Mathf.Atan2(targetPos.x - currentPos.x, targetPos.z - currentPos.z) * Mathf.Rad2Deg;
    }
    Vector3 currentPos => new Vector3(transform.position.x, 0.0f, transform.position.z);
    IEnumerator<float> Navigating(HexTilemapPath<MapTile> path, int stopTileDistance)
    {
        MapTile destination = path.route[path.route.Count - 1];
        foreach (var next in path.route)
        {
            if (Cubic.Distance(position, destination.position) <= stopTileDistance)
            {
                LookAt(new Vector3(destination.transform.position.x, 0.0f, destination.transform.position.z));
                StopNavigating();
                yield break;
            }
            Vector3 targetPos = new Vector3(next.transform.position.x, 0.0f, next.transform.position.z);
            LookAt(targetPos);
            while (Vector3.Distance(currentPos, targetPos) > stopDistance)
            {
                if (!next.isWalkable)
                {
                    StopNavigating();
                    yield break;
                }
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
        energy = Mathf.Max(energy - amount * energyConsuptionMultiplier, 0.0f);
    }
    readonly int isHoldingID = Animator.StringToHash("IsHolding");
    public void SetEquipment(EquipmentData equipmentData)
    {
        equipment = equipmentData.Create();
        equipment.transform.SetParent(sheatheAnchor);
        equipment.transform.localScale = Vector3.one;
        equipment.transform.localPosition = Vector3.zero;
        equipment.transform.localRotation = Quaternion.identity;
        anim.SetBool(isHoldingID, false);
    }
    public void EquipmentDamage(float damage)
    {
        if (equipment == null) return;
        equipment.LoseDurability(damage * durabilityConsuptionMultiplier);
        if(equipment == null) anim.SetBool(isHoldingID, false);
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
        public override void OnStateUpdate()
        {
            if (origin.home != null && origin.home.self == null) origin.home = null;
            if (origin.workplace != null && origin.workplace.self == null) origin.workplace = null;
            base.OnStateUpdate();
        }
        protected abstract NPC_WorkLayer<T> GetState_WorkLayer();

        //Utility States
        protected class NPC_Navigate<T> : State<T> where T : NPC
        {
            readonly Func<Cubic> destinationGetter;
            readonly Func<int> stopTileDistanceGetter;
            readonly Action onArrival;
            public NPC_Navigate(T origin, Layer<T> parent, Func<Cubic> destinationGetter, Func<int> stopTileDistanceGetter, Action onArrival) : base(origin, parent)
            {
                this.destinationGetter = destinationGetter;
                this.stopTileDistanceGetter = stopTileDistanceGetter;
                this.onArrival = onArrival;
            }
            const float pathSearchRate = 5.0f;
            float counter = 0.0f;
            public override void OnStateEnter()
            {
                base.OnStateEnter();
                origin.Navigate(origin.assignedIsland.tilemap.FindPath<MapTile>(origin.position, destinationGetter.Invoke()), stopTileDistanceGetter.Invoke());
                counter = 0.0f;
            }
            public override void OnStateUpdate()
            {
                base.OnStateUpdate();
                if (!origin.isNavigating)
                {
                    if (Cubic.Distance(origin.position, destinationGetter.Invoke()) <= stopTileDistanceGetter.Invoke())
                    {
                        origin.StopNavigating(); onArrival?.Invoke(); return;
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
                        break;
                    }
                }
            }
        }
        protected abstract class NPC_WorkLayer<T> : Layer<T> where T : NPC
        {
            protected NPC_WorkLayer(T origin, Layer<T> parent, int stopDistance = 0) : base(origin, parent)
            {
                defaultState = new LookForWork<T>(origin, this);
                AddState("LookForWork", defaultState);
                AddState("GoToWork", new NPC_Navigate<T>(origin, this,
                    () => origin.workplace.self.position,
                    () => stopDistance,
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
            class LookForWork<T> : Layer<T> where T : NPC
            {
                public LookForWork(T origin, Layer<T> parent) : base(origin, parent)
                {
                    defaultState = new Searching<T>(origin, this);
                    AddState("Searching", defaultState);
                    AddState("Wandering", new Wandering<T>(origin, this));
                }
                class Searching<T> : State<T> where T : NPC
                {
                    public Searching(T origin, Layer<T> parent) : base(origin, parent)
                    {

                    }
                    const float waitTime = 20.0f;
                    float counter = 0.0f;
                    public override void OnStateEnter()
                    {
                        base.OnStateEnter();
                        counter = 0.0f;
                    }
                    public override void OnStateUpdate()
                    {
                        base.OnStateUpdate();
                        counter += Time.deltaTime;
                        if(counter >= waitTime)
                        {
                            parentLayer.ChangeState("Wandering"); return;
                        }
                    }
                }
                class Wandering<T> : State<T> where T : NPC
                {
                    public Wandering(T origin, Layer<T> parent) : base(origin, parent)
                    {

                    }
                    const float wanderRate = 10.0f;
                    const int wanderRange = 2;
                    float counter = 0.0f;
                    public override void OnStateEnter()
                    {
                        base.OnStateEnter();
                        counter = 0.0f;
                        Wander();
                    }
                    public override void OnStateUpdate()
                    {
                        base.OnStateUpdate();
                        counter += Time.deltaTime;
                        if(counter >= wanderRate)
                        {
                            counter = 0.0f;
                            Wander();
                        }
                    }
                    readonly List<Cubic> randomList = new();
                    void Wander()
                    {
                        randomList.Clear();
                        Cubic pos = origin.home.self.position;
                        for(int q = -wanderRange; q <= wanderRange; q++)
                        {
                            for(int r = Mathf.Max(-wanderRange, -q-wanderRange); r <= Mathf.Min(wanderRange, -q+wanderRange); r++)
                            {
                                randomList.Add(pos + new Cubic(q, r, -q - r));
                            }
                        }
                        origin.Navigate(origin.assignedIsland.tilemap.FindPath<MapTile>(origin.position, randomList[UnityEngine.Random.Range(0, randomList.Count)]));
                    }

                    public override void OnStateExit()
                    {
                        base.OnStateExit();
                        if (origin.isNavigating) origin.StopNavigating();
                    }
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
            protected abstract class NPC_Working<T> : Layer<T> where T : NPC
            {
                protected NPC_Working(T origin, Layer<T> parent) : base(origin, parent)
                {

                }
                public override void OnStateUpdate()
                {
                    if(origin.workplace == null)
                    {
                        parentLayer.ChangeState("LookForWork"); return;
                    }
                    base.OnStateUpdate();
                }
            }
        }
        class NPC_Resting<T> : Layer<T> where T : NPC
        {
            public NPC_Resting(T origin, Layer<T> parent) : base(origin, parent)
            {
                defaultState = new NPC_Navigate<T>(origin, this,
                    () => origin.home.self.position,
                    () => 0,
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