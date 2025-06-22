using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using HexKit3D;
using UnityEngine;

public class CraftingStation : Structure, IWorkplace
{
    [Header("Crafting Station")]
    [SerializeField] int m_levelRequirement = 0;
    [SerializeField] int m_maxWorkers = 3;
    public int levelRequirement => m_levelRequirement;
    public int maxWorkers => m_maxWorkers;
    public List<NPC> workers { get; } = new();
}