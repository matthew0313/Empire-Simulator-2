using UnityEngine;

public abstract class EquipmentData : ItemData
{
    [Header("Equipment")]
    [SerializeField] float m_durability;
    public float durability => m_durability;
    public abstract Equipment Create();
}
public abstract class Equipment
{
    public readonly EquipmentData data;
    public Equipment(EquipmentData data)
    {
        this.data = data;
        durabilityLeft = data.durability;
    }
    public float durabilityLeft;
}