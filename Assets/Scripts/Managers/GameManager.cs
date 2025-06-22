using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour, ISavable
{
    public static GameManager Instance { get; private set; }
    public GameManager() => Instance = this;
    [SerializeField] Island m_currentIsland;
    public Island currentIsland => m_currentIsland;

    [Header("Saving")]
    [SerializeField] PlacedMapElement[] placedMapElementPrefabs;

    [Header("NPC")]
    [SerializeField] float m_baseNPCMoveSpeed = 5.0f;
    [SerializeField] float m_npcNavigationStopDistance = 0.01f;
    [SerializeField] float m_npcRotateRate = 4.0f;
    [SerializeField] float m_npcHeightRate = 4.0f;
    [SerializeField] float m_npcHeight = 2.0f;
    [SerializeField] LayerMask m_obstructionLayer;
    public float baseNPCMoveSpeed => m_baseNPCMoveSpeed;
    public float npcNavigationStopDistance => m_npcNavigationStopDistance;
    public float npcRotateRate => m_npcRotateRate;
    public float npcHeightRate => m_npcHeightRate;
    public float npcHeight => m_npcHeight;
    public LayerMask obstructionLayer => m_obstructionLayer;

    [Header("Time")]
    [SerializeField] Transform sunAnchor;
    [SerializeField] float m_workStartTime = 7.0f;
    [SerializeField] float m_workEndTime = 22.0f;
    [SerializeField] float hourSecondsRatio = 60.0f;
    public float workStartTime => m_workStartTime;
    public float workEndTime => m_workEndTime;
    [field:SerializeField]
    public float currentTime { get; private set; } = 0.0f;
    public bool isWorkTime => currentTime > workStartTime && currentTime < workEndTime;
    [field: SerializeField]
    public int currentDay { get; private set; } = 1;
    public Season currentSeason
    {
        get
        {
            if (currentDay < 4) return Season.Spring;
            if (currentDay < 7) return Season.Summer;
            if (currentDay < 10) return Season.Fall;
            else return Season.Winter;
        }
    }
    public Action onDayChange;

    [Header("BuildMode")]
    [SerializeField] Material m_canPlaceMaterial;
    [SerializeField] Material m_cannotPlaceMaterial;
    public Material canPlaceMaterial => m_canPlaceMaterial;
    public Material cannotPlaceMaterial => m_cannotPlaceMaterial;

    [Header("Unlocks")]
    [SerializeField] Blueprint[] startingBlueprints;
    public readonly List<Blueprint> unlockedBlueprints = new();


    [Header("Debug")]
    [SerializeField] string buildMode_FSMPath;
    BuildMode_TopLayer buildMode_TopLayer;
    void Awake()
    {
        buildMode_TopLayer = new(this);
#if UNITY_EDITOR
        buildMode_TopLayer.onFSMChange += () => { buildMode_FSMPath = buildMode_TopLayer.GetFSMPath(); };
#endif
        buildMode_TopLayer.OnStateEnter();
    }
    private void Start()
    {
        if (!loaded)
        {
            foreach (var i in startingBlueprints) UnlockBlueprint(i);
        }
    }
    private void Update()
    {
        UpdateTime();
        buildMode_TopLayer.OnStateUpdate();
    }
    void UpdateTime()
    {
        currentTime += Time.deltaTime * (1.0f / hourSecondsRatio);
        if (currentTime > 24.0f)
        {
            currentTime = 0.0f;
            currentDay++;
            if (currentDay > 12) currentDay = 1;
            onDayChange?.Invoke();
        }

        sunAnchor.rotation = Quaternion.Euler(-90.0f + currentTime / 24.0f * 360.0f, 30.0f, 0.0f);
    }
    public void SwitchIsland(Island island)
    {
        if (island == currentIsland) return;
    }
    public void UnlockBlueprint(Blueprint blueprint)
    {
        unlockedBlueprints.Add(blueprint);
        onBlueprintUnlock?.Invoke(blueprint);
    }
    public Action<Blueprint> onBlueprintUnlock;
    public PlacedMapElement GetPlacedMapElementPrefab(string id)
    {
        foreach(var i in placedMapElementPrefabs)
        {
            if (i.id == id) return i;
        }
        return null;
    }

    public BuildMode_Mode buildMode = BuildMode_Mode.None;
    Blueprint placingBlueprint = null;
    public void EnterBuildMode(Blueprint blueprint)
    {
        placingBlueprint = blueprint;
        buildMode = BuildMode_Mode.Place;
    }
    public void EnterDestroyMode() => buildMode = BuildMode_Mode.Destroy;
    public void ExitBuildMode() => buildMode = BuildMode_Mode.None;
    public void Save(SaveData data)
    {
        foreach(var i in unlockedBlueprints)
        {
            data.unlockedBlueprints.Add(i.id);
        }
    }
    bool loaded = false;
    public void Load(SaveData data)
    {
        loaded = true;
        foreach(var i in data.unlockedBlueprints)
        {
            if (GetPlacedMapElementPrefab(i) is Blueprint blueprint) UnlockBlueprint(blueprint);
        }
    }
    class BuildMode_TopLayer : TopLayer<GameManager>
    {
        public BuildMode_TopLayer(GameManager origin) : base(origin, null)
        {
            defaultState = new Idle(origin, this);
            AddState("Idle", defaultState);
            AddState("BuildMode", new BuildMode(origin, this));
        }
        class Idle : State<GameManager>
        {
            public Idle(GameManager origin, Layer<GameManager> parent) : base(origin, parent) { }
            public override void OnStateUpdate()
            {
                if (origin.buildMode != BuildMode_Mode.None)
                {
                    parentLayer.ChangeState("BuildMode"); return;
                }
                base.OnStateUpdate();
            }
        }
        class BuildMode : Layer<GameManager>
        {
            public BuildMode(GameManager origin, Layer<GameManager> parent) : base(origin, parent)
            {
                AddState("Placing", new Placing(origin, this));
                AddState("Destroying", new Destroying(origin, this));
            }
            public override void OnStateEnter()
            {
                if (currentState != null) currentState.OnStateExit();
                if(origin.buildMode == BuildMode_Mode.None)
                {
                    parentLayer.ChangeState("Idle"); return;
                }
                switch (origin.buildMode)
                {
                    case BuildMode_Mode.Place: currentState = states["Placing"]; break;
                    case BuildMode_Mode.Destroy: currentState = states["Destroying"]; break;
                }
                currentState.OnStateEnter();

                UIManager.Instance.defaultUI.Hide();
                UIManager.Instance.buildUI.Show();
            }
            public override void OnStateUpdate()
            {
                if (Input.GetKeyDown(KeyCode.Escape)) origin.buildMode = BuildMode_Mode.None;
                if(origin.buildMode == BuildMode_Mode.None)
                {
                    parentLayer.ChangeState("Idle"); return;
                }
                base.OnStateUpdate();
            }
            public override void OnStateExit()
            {
                base.OnStateExit();

                UIManager.Instance.defaultUI.Show();
                UIManager.Instance.buildUI.Hide();
            }
            class Placing : State<GameManager>
            {
                public Placing(GameManager origin, Layer<GameManager> parent) : base(origin, parent) { }
                Blueprint placing;
                bool positioned = false;
                float rotation = 0.0f;
                public override void OnStateEnter()
                {
                    base.OnStateEnter();
                    if (origin.placingBlueprint == null)
                    {
                        origin.buildMode = BuildMode_Mode.None;
                        parentLayer.parentLayer.ChangeState("Idle");
                        return;
                    }
                    placing = Instantiate(origin.placingBlueprint);
                    placing.EnterPlaceMode();
                    placing.transform.rotation = Quaternion.Euler(0, rotation, 0);
                    EmpireManager.Instance.onInventoryUpdate += OnInventoryUpdate;
                    positioned = false;
                    UIManager.Instance.buildUI.placeUI.Show();
                    CheckIngredients();
                }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();
                    if(origin.buildMode != BuildMode_Mode.Place)
                    {
                        parentLayer.OnStateEnter(); return;
                    }


                    if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000.0f, LayerMask.GetMask("Tile")))
                    {
                        if(hit.transform.TryGetComponent(out MapTile tile))
                        {
                            placing.transform.position = tile.transform.position;
                            positioned = true;
                        }
                    }

                    placing.cannotPlace = false;
                    if (!enoughIngredients)
                    {
                        placing.cannotPlace = true;
                    }
                    if (placing.isObstructed)
                    {

                    }

                    if (positioned && Input.GetMouseButtonDown(0)) Place();
                    if (Input.GetKeyDown(KeyCode.R)) Rotate();
                }
                void OnInventoryUpdate(ItemData item)
                {
                    foreach(var i in placing.ingredients)
                    {
                        if (i.item == item) CheckIngredients();
                    }
                }
                bool enoughIngredients = false;
                void CheckIngredients()
                {
                    foreach (var i in placing.ingredients)
                    {
                        if (EmpireManager.Instance.GetItemCount(i.item) < i.count)
                        {
                            enoughIngredients = false; return;
                        }
                    }
                    enoughIngredients = true;
                }
                void Place()
                {
                    if (!placing.canPlace) return;
                    placing.ExitPlaceMode();
                    origin.currentIsland.PlaceElement(placing);
                    foreach(var i in placing.ingredients)
                    {
                        EmpireManager.Instance.RemoveItem(i.item, i.count);
                    }
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        placing = Instantiate(origin.placingBlueprint);
                    }
                    else
                    {
                        placing = null;
                        Exit();
                    }
                }
                void Rotate()
                {
                    rotation += 60.0f;
                    placing.transform.rotation = Quaternion.Euler(0, rotation, 0);
                }
                void Exit()
                {
                    origin.buildMode = BuildMode_Mode.None;
                    parentLayer.parentLayer.ChangeState("Idle");
                }
                public override void OnStateExit()
                {
                    base.OnStateExit();
                    EmpireManager.Instance.onInventoryUpdate -= OnInventoryUpdate;
                    if (placing != null) Destroy(placing.gameObject);

                    UIManager.Instance.buildUI.placeUI.Hide();
                }
            }
            class Destroying : State<GameManager>
            {
                public Destroying(GameManager origin, Layer<GameManager> parent) : base(origin, parent) { }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();
                    if (origin.buildMode != BuildMode_Mode.Destroy)
                    {
                        parentLayer.OnStateEnter(); return;
                    }

                    if (Input.GetMouseButtonDown(0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000.0f, LayerMask.GetMask("MapElement")))
                    {
                        if (hit.transform.TryGetComponent(out PlacedMapElement element))
                        {
                            origin.currentIsland.RemoveElement(element);
                            Destroy(element.gameObject);
                        }
                    }
                }
            }
        }
    }
}
[System.Serializable]
public enum Season
{
    Spring = 0,
    Summer = 1,
    Fall = 2,
    Winter = 3
}
public enum BuildMode_Mode
{
    None = 0,
    Place = 1,
    Destroy = 2
}
#if UNITY_EDITOR
[CustomEditor(typeof(GameManager))]
public class GameManager_Editor : Editor
{
    Blueprint blueprint;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(30);
        blueprint = EditorGUILayout.ObjectField(blueprint, typeof(Blueprint), false) as Blueprint;
        if(GUILayout.Button("Enter Build Mode") && Application.isPlaying && blueprint != null)
        {
            (target as GameManager).EnterBuildMode(blueprint);
        }
    }
}
#endif