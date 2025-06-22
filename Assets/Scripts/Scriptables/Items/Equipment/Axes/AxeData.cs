using UnityEngine;

public abstract class AxeData : EquipmentData
{
    [Header("Axe")]
    [SerializeField] float m_damage;
    public float damage => m_damage;
}