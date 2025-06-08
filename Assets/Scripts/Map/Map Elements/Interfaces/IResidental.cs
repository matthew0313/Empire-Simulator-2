using System.Collections.Generic;
using HexKit3D;
using UnityEngine;

public interface IResidental : IMapElement
{
    public int maxResidents { get; }
    public List<NPC> residents { get; }
    public float energyGiven { get; }
}