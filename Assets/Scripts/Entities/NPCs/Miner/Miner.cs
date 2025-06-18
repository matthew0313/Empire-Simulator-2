using System;
using HexKit3D;
using UnityEngine;
using MEC;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.InputSystem.Android;
using Unity.VisualScripting;
using System.Net.NetworkInformation;

public class Miner : NPC
{
    [Header("Lumberjack")]
    [SerializeField] UnitTime mineTime;

    [Header("Debug")]
    [SerializeField] string FSMPath;

    Miner_TopLayer topLayer;

    OreNode m_selectedOre = null;
    OreNode selectedOre
    {
        get => m_selectedOre;
        set
        {
            if(m_selectedOre != value)
            {
                if (m_selectedOre != null) m_selectedOre.queuedMiner = null;
                m_selectedOre = value;
                if (m_selectedOre != null) m_selectedOre.queuedMiner = this;
            }
        }
    }
    private void Awake()
    {
        topLayer = new(this, new NPC_FSMVals());
        topLayer.OnStateEnter();
#if UNITY_EDITOR
        topLayer.onFSMChange += () => FSMPath = topLayer.GetFSMPath();
        FSMPath = topLayer.GetFSMPath();
#endif
    }
    protected override void Update()
    {
        base.Update();
        topLayer.OnStateUpdate();
    }
    private void FixedUpdate()
    {
        topLayer.OnStateFixedUpdate();
    }
    private void OnDestroy()
    {
        topLayer.OnStateExit();
    }

    readonly List<IWorkplace> availableJobsList = new();
    protected override List<IWorkplace> GetAvailableJobs()
    {
        availableJobsList.Clear();
        foreach(var i in assignedIsland.SearchElements(element => element is MinerHut))
        {
            if((i as MinerHut).levelRequirement <= level) availableJobsList.Add(i as IWorkplace);
        }
        return availableJobsList;
    }
    float mineRate => 1.0f;
    float energyPerMine => 1.0f;
    float durabilityPerMine => 1.0f;
    float damageMultiplier => 1.0f;
    class Miner_TopLayer : NPC_TopLayer<Miner>
    {
        public Miner_TopLayer(Miner origin, NPC_FSMVals values) : base(origin, values) { }

        protected override NPC_WorkLayer<Miner> GetState_WorkLayer() => new Miner_WorkLayer(origin, this);
        class Miner_WorkLayer : NPC_WorkLayer<Miner>
        {
            public Miner_WorkLayer(Miner origin, Layer<Miner> parent) : base(origin, parent)
            {
                defaultState = new NPC_SearchForTools<Miner, PickaxeData>(origin, this);
                AddState("SearchForTools", defaultState);
            }
            protected override NPC_Working<Miner> GetState_Working() => new Miner_Working(origin, this);
            class Miner_Working : NPC_Working<Miner>
            {
                public Miner_Working(Miner origin, Layer<Miner> parent) : base(origin, parent)
                {
                    defaultState = new SearchOre(origin, this);
                    AddState("SearchOre", defaultState);
                    AddState("MoveToOre", new MoveToOre(origin, this));
                    AddState("MineOre", new MineOre(origin, this));
                }
                public override void OnStateEnter()
                {
                    base.OnStateEnter();
                    origin.UnSheathe();
                }
                public override void OnStateUpdate()
                {
                    if (origin.selectedOre != null && !origin.selectedOre.available) origin.selectedOre = null;
                    if (origin.equipment == null)
                    {
                        parentLayer.ChangeState("SearchForTools"); return;
                    }
                    base.OnStateUpdate();
                }
                public override void OnStateExit()
                {
                    base.OnStateExit();
                    origin.Sheathe();
                }
                class SearchOre : State<Miner>
                {
                    public SearchOre(Miner origin, Layer<Miner> parent) : base(origin, parent) { }
                    const float searchRate = 1.0f;
                    float counter = 0.0f;
                    public override void OnStateEnter()
                    {
                        base.OnStateEnter();
                        if (origin.selectedOre != null)
                        {
                            parentLayer.ChangeState("MoveToOre"); return;
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
                            counter = 0.00f;
                            Search();
                        }
                        if(origin.selectedOre != null)
                        {
                            parentLayer.ChangeState("MoveToOre"); return;
                        }
                    }
                    void Search()
                    {
                        var list = (origin.workplace as MinerHut).SearchOres();
                        list.RemoveAll(node => !node.available || node.requiredTier > (origin.equipment as Pickaxe).data.tier || node.queuedMiner != null);
                        if(list.Count > 0)
                        {
                            origin.selectedOre = list[0];
                        }
                    }
                }
                class MoveToOre : NPC_Navigate<Miner>
                {
                    public MoveToOre(Miner origin, Layer<Miner> parent) : base(origin, parent,
                        () => origin.selectedOre.position,
                        () => 1,
                        () => parent.ChangeState("MineOre"))
                    {

                    }
                    public override void OnStateUpdate()
                    {
                        if(origin.selectedOre == null)
                        {
                            parentLayer.ChangeState("SearchOre"); return;
                        }
                        base.OnStateUpdate();
                    }
                }
                class MineOre : State<Miner>
                {
                    public MineOre(Miner origin, Layer<Miner> parent) : base(origin, parent) { }
                    float counter = 0.0f;
                    public override void OnStateEnter()
                    {
                        base.OnStateEnter();
                        if(origin.selectedOre == null)
                        {
                            parentLayer.ChangeState("SearchOre"); return;
                        }
                        counter = 0.0f;
                        origin.LookAt(origin.selectedOre.transform.position);
                    }
                    public override void OnStateUpdate()
                    {
                        base.OnStateUpdate();
                        if (origin.selectedOre == null)
                        {
                            parentLayer.ChangeState("SearchOre"); return;
                        }
                        counter += Time.deltaTime;
                        if(counter >= origin.mineRate)
                        {
                            Mine();
                            counter = 0.0f;
                        }
                    }
                    CoroutineHandle mining;
                    public override void OnStateExit()
                    {
                        base.OnStateExit();
                        Timing.KillCoroutines(mining);
                    }
                    readonly int mineID = Animator.StringToHash("Mine");
                    void Mine()
                    {
                        origin.anim.SetTrigger(mineID);
                        mining = Timing.RunCoroutine(CoroutineUtility.WaitThen(origin.mineTime, () =>
                        {
                            origin.selectedOre.GetDamage((origin.equipment as Pickaxe).data.damage * origin.damageMultiplier);
                            origin.EquipmentDamage(origin.durabilityPerMine);
                            origin.LoseEnergy(origin.energyPerMine);
                        }));
                    }
                }
            }
        }
    }
}