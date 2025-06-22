using UnityEngine;

public abstract class SickleData : EquipmentData
{
    [Header("Pickaxe")]
    [SerializeField] float m_growth;
    public float growth => m_growth;
}