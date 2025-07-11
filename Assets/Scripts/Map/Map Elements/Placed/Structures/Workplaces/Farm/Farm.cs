using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using HexKit3D;
using UnityEngine;

public class Farm : Structure, IWorkplace
{
    public override StructureCategory category => base.category | StructureCategory.Workplace | StructureCategory.Agriculture;

    [Header("Farm")]
    [SerializeField] LootTable loot;
    [SerializeField] Animator anim;
    [SerializeField] int m_maxWorkers = 1;
    [SerializeField] int m_levelRequirement = 1;
    [SerializeField] float m_growthRequired = 100;
    [SerializeField] ParticleSystem growthParticle, harvestParticle;

    public ItemData crop => loot.primaryLoot;
    public int maxWorkers => m_maxWorkers;
    public int levelRequirement => m_levelRequirement;
    public float growthRequired => m_growthRequired;
    public List<NPC> workers { get; } = new();
    public float growth { get; private set; }
    public Action onGrowthChange;

    readonly int harvestID = Animator.StringToHash("Harvest");
    readonly int growthID = Animator.StringToHash("Growth");
    public void AddGrowth(float amount)
    {
        growth += amount;
        if (growth >= growthRequired)
        {
            anim.SetTrigger(harvestID);
            foreach (var i in loot.GetLoot()) EmpireManager.Instance.AddItem(i.item, i.count);
            growth = 0.0f;
            harvestParticle.Play();
        }
        else growthParticle.Play();
        anim.SetFloat(growthID, growth / growthRequired);
        onGrowthChange?.Invoke();
    }
}