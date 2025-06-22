using UnityEngine;

public abstract class EquipmentData : ItemData
{
    [Header("Equipment")]
    [SerializeField] float m_durability = 100;
    [SerializeField] int m_tier = 0;
    public float durability => m_durability;
    public int tier => m_tier;
    public override LangText itemDesc => base.itemDesc + new LangText()
    {
        en = $"\n\nTier: {tier}\nDurability:{durability}",
        kr = $"\n\n티어: {tier}\n내구도: {durability}"
    };
    public abstract Equipment Create();
}