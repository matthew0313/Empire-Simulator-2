using System.Collections.Generic;
using HexKit3D;
using UnityEngine;

public interface IResidental
{
    public MapElement self { get; }
    public int maxResidents { get; }
    public List<NPC> residents { get; }
    public float energyGiven { get; }
}