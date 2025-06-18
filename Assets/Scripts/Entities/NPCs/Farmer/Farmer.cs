using System;
using HexKit3D;
using UnityEngine;
using MEC;
using System.Collections.Generic;

public class Farmer : NPC
{
    [Header("Miner")]
    [SerializeField] UnitTime farmTime;

    [Header("Debug")]
    [SerializeField] string FSMPath;
    Farmer_TopLayer topLayer;
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
        foreach (var i in assignedIsland.SearchElements(element => element is Farm))
        {
            if ((i as Farm).levelRequirement <= level) availableJobsList.Add(i as IWorkplace);
        }
        return availableJobsList;
    }
    float farmRate => 1.0f;
    float energyPerFarm => 1.0f;
    float durabilityPerFarm => 1.0f;
    float growthMultiplier = 1.0f;
    class Farmer_TopLayer : NPC_TopLayer<Farmer>
    {
        public Farmer_TopLayer(Farmer origin, NPC_FSMVals values) : base(origin, values)
        {

        }

        protected override NPC_WorkLayer<Farmer> GetState_WorkLayer() => new Farmer_WorkLayer(origin, this);
        class Farmer_WorkLayer : NPC_WorkLayer<Farmer>
        {
            public Farmer_WorkLayer(Farmer origin, Layer<Farmer> parent) : base(origin, parent)
            {
                defaultState = new NPC_SearchForTools<Farmer, SickleData>(origin, this);
                AddState("SearchForTools", defaultState);
            }

            protected override NPC_Working<Farmer> GetState_Working() => new Farmer_Working(origin, this);
            class Farmer_Working : NPC_Working<Farmer>
            {
                public Farmer_Working(Farmer origin, Layer<Farmer> parent) : base(origin, parent)
                {

                }
                readonly int isCrouchingID = Animator.StringToHash("IsCrouching");
                readonly int farmID = Animator.StringToHash("Farm");
                float counter = 0.0f;
                public override void OnStateEnter()
                {
                    base.OnStateEnter();
                    origin.anim.SetBool(isCrouchingID, true);
                    counter = 0.0f;
                }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();
                    counter += Time.deltaTime;
                    if(counter >= origin.farmRate)
                    {
                        Farm();
                        counter = 0.0f;
                    }
                }
                CoroutineHandle farming;
                public override void OnStateExit()
                {
                    base.OnStateExit();
                    origin.anim.SetBool(isCrouchingID, false);
                    Timing.KillCoroutines(farming);
                }
                void Farm()
                {
                    origin.anim.SetTrigger(farmID);
                    farming = Timing.RunCoroutine(CoroutineUtility.WaitThen(origin.farmTime, () =>
                    {
                        (origin.workplace as Farm).AddGrowth((origin.equipment as Sickle).data.growth * origin.growthMultiplier);
                        origin.EquipmentDamage(origin.durabilityPerFarm);
                        origin.LoseEnergy(origin.energyPerFarm);
                    }));
                }
            }
        }
    }
}