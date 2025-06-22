using UnityEngine;

public abstract class SickleData : EquipmentData
{
    [Header("Pickaxe")]
    [SerializeField] float m_growth;
    [SerializeField] float m_energyPerUse = 1.0f;
    public float growth => m_growth;
    public float energyPerUse => m_energyPerUse;
}