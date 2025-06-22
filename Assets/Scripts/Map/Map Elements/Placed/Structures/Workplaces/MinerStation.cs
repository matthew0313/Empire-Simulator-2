using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using HexKit3D;
using UnityEngine;

public class MinerStation : Structure, IWorkplace
{
    public override StructureCategory category => base.category | StructureCategory.Workplace;

    [Header("Miner Station")]
    [SerializeField] int m_maxWorkers = 1;
    [SerializeField] int m_levelRequirement = 1;
    [SerializeField] int m_range = 1;
    public int maxWorkers => m_maxWorkers;
    public int levelRequirement => m_levelRequirement;
    public int range => m_range;
    public List<NPC> workers { get; } = new();
    readonly List<OreNode> searchOreList = new();
    public List<OreNode> SearchOres()
    {
        searchOreList.Clear();
        foreach (var i in placedIsland.SearchElements(element => element is OreNode))
        {
            if(Cubic.Distance(i.position, position) <= range) searchOreList.Add(i as OreNode);
        }
        return searchOreList;
    }
}