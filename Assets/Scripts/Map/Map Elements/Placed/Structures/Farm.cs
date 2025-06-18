using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using HexKit3D;
using UnityEngine;

public class Farm : Structure, IWorkplace
{
    [Header("Farm")]
    [SerializeField] Animator anim;
    [SerializeField] int m_maxWorkers = 1;
    [SerializeField] int m_levelRequirement = 1;
    [SerializeField] float growthRequired = 100;
    [SerializeField] LootTable harvest;
    float growth = 0.0f;
    public int maxWorkers => m_maxWorkers;
    public int levelRequirement => m_levelRequirement;
    public List<NPC> workers { get; } = new();

    readonly int harvestID = Animator.StringToHash("Harvest");
    readonly int growthID = Animator.StringToHash("Growth");
    public void AddGrowth(float amount)
    {
        growth += amount;
        if(growth >= growthRequired)
        {
            anim.SetTrigger(harvestID);
            foreach (var i in harvest.GetLoot()) EmpireManager.Instance.AddItem(i.item, i.count);
            growth = 0.0f;
        }
        anim.SetFloat(growthID, growth / growthRequired);
    }
}