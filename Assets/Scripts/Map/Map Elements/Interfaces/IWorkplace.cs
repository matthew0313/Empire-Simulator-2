using System.Collections.Generic;
using HexKit3D;
using UnityEngine;

public interface IWorkplace : IMapElement
{
    public int levelRequirement { get; }
    public int maxWorkers { get; }
    public List<NPC> workers { get; }
}