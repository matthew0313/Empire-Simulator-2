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

public class Lumberjack : NPC
{
    [Header("Debug")]
    [SerializeField] string FSMPath;

    Lumberjack_TopLayer topLayer;

    TreeNode m_selectedTree = null;
    TreeNode selectedTree
    {
        get => m_selectedTree;
        set
        {
            if(m_selectedTree != value)
            {
                if (m_selectedTree != null) m_selectedTree.queuedLumberjack = null;
                m_selectedTree = value;
                if (m_selectedTree != null) m_selectedTree.queuedLumberjack = this;
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
        foreach(var i in assignedIsland.SearchElements(element => element is LumberjackHut))
        {
            if((i as LumberjackHut).levelRequirement <= level) availableJobsList.Add(i as IWorkplace);
        }
        return availableJobsList;
    }
    float chopRate => 1.0f;
    float energyPerChop => 1.0f;
    float durabilityPerChop => 1.0f;
    float damageMultiplier => 1.0f;
    class Lumberjack_TopLayer : NPC_TopLayer<Lumberjack>
    {
        public Lumberjack_TopLayer(Lumberjack origin, NPC_FSMVals values) : base(origin, values) { }

        protected override NPC_WorkLayer<Lumberjack> GetState_WorkLayer() => new Lumberjack_WorkLayer(origin, this);
        class Lumberjack_WorkLayer : NPC_WorkLayer<Lumberjack>
        {
            public Lumberjack_WorkLayer(Lumberjack origin, Layer<Lumberjack> parent) : base(origin, parent)
            {
                defaultState = new NPC_SearchForTools<Lumberjack, AxeData>(origin, this);
                AddState("SearchForTools", defaultState);
            }
            protected override NPC_Working<Lumberjack> GetState_Working() => new Lumberjack_Working(origin, this);
            class Lumberjack_Working : NPC_Working<Lumberjack>
            {
                public Lumberjack_Working(Lumberjack origin, Layer<Lumberjack> parent) : base(origin, parent)
                {
                    defaultState = new SearchTree(origin, this);
                    AddState("SearchTree", defaultState);
                    AddState("MoveToTree", new MoveToTree(origin, this));
                    AddState("ChopTree", new ChopTree(origin, this));
                }
                public override void OnStateEnter()
                {
                    base.OnStateEnter();
                    origin.UnSheathe();
                }
                public override void OnStateUpdate()
                {
                    if (!origin.selectedTree.available) origin.selectedTree = null;
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
                class SearchTree : State<Lumberjack>
                {
                    public SearchTree(Lumberjack origin, Layer<Lumberjack> parent) : base(origin, parent) { }
                    const float searchRate = 1.0f;
                    float counter = 0.0f;
                    public override void OnStateEnter()
                    {
                        base.OnStateEnter();
                        if (origin.selectedTree != null)
                        {
                            parentLayer.ChangeState("MoveToTree"); return;
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
                        if(origin.selectedTree != null)
                        {
                            parentLayer.ChangeState("MoveToTree"); return;
                        }
                    }
                    void Search()
                    {
                        var list = (origin.workplace as LumberjackHut).SearchTrees();
                        list.RemoveAll(node => !node.available || node.requiredTier > (origin.equipment as Axe).data.tier || node.queuedLumberjack != null);
                        if(list.Count > 0)
                        {
                            origin.selectedTree = list[0];
                        }
                    }
                }
                class MoveToTree : NPC_Navigate<Lumberjack>
                {
                    public MoveToTree(Lumberjack origin, Layer<Lumberjack> parent) : base(origin, parent,
                        () => origin.selectedTree.position,
                        () => 1,
                        () => parent.ChangeState("ChopTree"))
                    {

                    }
                    public override void OnStateUpdate()
                    {
                        if(origin.selectedTree == null)
                        {
                            parentLayer.ChangeState("SearchTree"); return;
                        }
                        base.OnStateUpdate();
                    }
                }
                class ChopTree : State<Lumberjack>
                {
                    public ChopTree(Lumberjack origin, Layer<Lumberjack> parent) : base(origin, parent) { }
                    float counter = 0.0f;
                    public override void OnStateEnter()
                    {
                        base.OnStateEnter();
                        if(origin.selectedTree == null)
                        {
                            parentLayer.ChangeState("SearchTree"); return;
                        }
                        counter = 0.0f;
                    }
                    public override void OnStateUpdate()
                    {
                        base.OnStateUpdate();
                        if (origin.selectedTree == null)
                        {
                            parentLayer.ChangeState("SearchTree"); return;
                        }
                        counter += Time.deltaTime;
                        if(counter >= origin.chopRate)
                        {
                            Chop();
                            counter = 0.0f;
                        }
                    }
                    readonly int chopID = Animator.StringToHash("Chop");
                    void Chop()
                    {
                        origin.anim.SetTrigger(chopID);
                        origin.selectedTree.GetDamage((origin.equipment as Axe).data.damage * origin.damageMultiplier);
                        origin.equipment.LoseDurability(origin.durabilityPerChop);
                        origin.LoseEnergy(origin.energyPerChop);
                    }
                }
            }
        }
    }
}