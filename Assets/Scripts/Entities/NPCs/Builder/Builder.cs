using System;
using HexKit3D;
using UnityEngine;
using MEC;
using System.Collections.Generic;

public class Builder : NPC
{
    [Header("Builder")]
    [SerializeField] UnitTime buildTime;
    [SerializeField] Transform hammer;

    [Header("Debug")]
    [SerializeField] string FSMPath;
    Builder_TopLayer topLayer;
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
        foreach (var i in assignedIsland.SearchElements(element => element is Blueprint))
        {
            if ((i as Blueprint).levelRequirement <= level) availableJobsList.Add(i as IWorkplace);
        }
        return availableJobsList;
    }
    float farmRate => 1.0f;
    float progressAmount => 1.0f;
    float energyPerProgress => 1.0f;
    class Builder_TopLayer : NPC_TopLayer<Builder>
    {
        public Builder_TopLayer(Builder origin, NPC_FSMVals values) : base(origin, values)
        {

        }

        protected override NPC_WorkLayer<Builder> GetState_WorkLayer() => new Builder_WorkLayer(origin, this);
        class Builder_WorkLayer : NPC_WorkLayer<Builder>
        {
            public Builder_WorkLayer(Builder origin, Layer<Builder> parent) : base(origin, parent)
            {

            }

            protected override NPC_Working<Builder> GetState_Working() => new Builder_Working(origin, this);
            class Builder_Working : NPC_Working<Builder>
            {
                public Builder_Working(Builder origin, Layer<Builder> parent) : base(origin, parent)
                {
                    defaultState = new Building(origin, this);
                    AddState("Building", defaultState);
                }
                readonly int isHoldingID = Animator.StringToHash("IsHolding");
                public override void OnStateEnter()
                {
                    base.OnStateEnter();
                    origin.hammer.transform.SetParent(origin.heldAnchor);
                    origin.hammer.transform.localScale = Vector3.one;
                    origin.hammer.transform.localPosition = Vector3.zero;
                    origin.hammer.transform.localRotation = Quaternion.identity;
                    origin.anim.SetBool(isHoldingID, true);
                }
                public override void OnStateExit()
                {
                    base.OnStateExit();
                    origin.hammer.transform.SetParent(origin.sheatheAnchor);
                    origin.hammer.transform.localScale = Vector3.one;
                    origin.hammer.transform.localPosition = Vector3.zero;
                    origin.hammer.transform.localRotation = Quaternion.identity;
                    origin.anim.SetBool(isHoldingID, false);
                }
                class Building : State<Builder>
                {
                    public Building(Builder origin, Layer<Builder> parent) : base(origin, parent) { }
                    readonly int buildID = Animator.StringToHash("Build");
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
                        if (counter >= origin.farmRate)
                        {
                            Build();
                            counter = 0.0f;
                        }
                    }
                    CoroutineHandle building;
                    public override void OnStateExit()
                    {
                        base.OnStateExit();
                        Timing.KillCoroutines(building);
                    }
                    void Build()
                    {
                        origin.anim.SetTrigger(buildID);
                        building = Timing.RunCoroutine(CoroutineUtility.WaitThen(origin.buildTime, () =>
                        {
                            (origin.workplace as Blueprint).AddProgress(origin.progressAmount * origin.workAmountMultiplier);
                            origin.LoseEnergy(origin.energyPerProgress);
                        }));
                    }
                }
            }
        }
    }
}