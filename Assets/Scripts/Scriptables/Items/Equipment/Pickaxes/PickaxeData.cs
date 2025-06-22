using UnityEngine;

public abstract class PickaxeData : EquipmentData
{
    [Header("Pickaxe")]
    [SerializeField] float m_damage;
    public float damage => m_damage;
}