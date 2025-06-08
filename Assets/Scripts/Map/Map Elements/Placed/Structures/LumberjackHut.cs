using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using HexKit3D;
using UnityEngine;

public class LumberjackHut : Structure, IWorkplace
{
    [Header("Lumberjack Hut")]
    [SerializeField] int m_maxWorkers = 1;
    [SerializeField] int m_levelRequirement = 1;
    [SerializeField] int m_range = 1;
    public int maxWorkers => m_maxWorkers;
    public int levelRequirement => m_levelRequirement;
    public int range => m_range;
    public List<NPC> workers { get; } = new();
    readonly List<TreeNode> searchTreeList = new();
    public List<TreeNode> SearchTrees()
    {
        searchTreeList.Clear();
        foreach (var i in placedIsland.SearchElements(element => element is TreeNode))
        {
            if(Cubic.Distance(i.position, position) <= range) searchTreeList.Add(i as TreeNode);
        }
        return searchTreeList;
    }
}