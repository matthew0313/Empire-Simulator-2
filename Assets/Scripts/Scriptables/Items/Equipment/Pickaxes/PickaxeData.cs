using UnityEngine;

public abstract class PickaxeData : EquipmentData
{
    [Header("Pickaxe")]
    [SerializeField] float m_damage;
    [SerializeField] float m_energyPerUse = 1.0f;
    public float damage => m_damage;
    public float energyPerUse => m_energyPerUse;
}