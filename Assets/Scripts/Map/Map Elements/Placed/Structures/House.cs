using System.Collections.Generic;
using HexKit3D;
using UnityEngine;

public class House : Structure, IResidental
{
    public MapElement self => this;

    [Header("House")]
    [SerializeField] int m_maxResidents;
    [SerializeField] float m_energyGiven;
    public int maxResidents => m_maxResidents;
    public List<NPC> residents { get; } = new();
    public float energyGiven => m_energyGiven;
}