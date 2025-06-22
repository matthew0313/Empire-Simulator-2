using UnityEngine;

[CreateAssetMenu(fileName = "Item Data", menuName = "Scriptables/Item/Item")]
public class ItemData : ScriptableObject
{
    [SerializeField] LangText m_itemName;
    [SerializeField] LangText m_itemDesc;
    [SerializeField] ItemCategory m_category;
    [SerializeField] Sprite m_itemIcon;
    public LangText itemName => m_itemName;
    public virtual LangText itemDesc => m_itemDesc;
    public ItemCategory category => m_category;
    public Sprite itemIcon => m_itemIcon;
    public static LangText CategoryToText(ItemCategory category)
    {
        switch (category)
        {
            case ItemCategory.Equipment: return new()
            {
                en = "Equipment",
                kr = "장비"
            };
            case ItemCategory.Food: return new()
            {
                en = "Food",
                kr = "음식"
            };
            case ItemCategory.Resource: return new()
            {
                en = "Resource",
                kr = "자원"
            };
        }
        return new();
    }
}
[System.Serializable]
public struct ItemIntPair
{
    public ItemData item;
    public int count;
}
[System.Serializable]
[System.Flags]
public enum ItemCategory
{
    Equipment = 1<<0,
    Food = 1<<1,
    Resource = 1<<2
}