using UnityEngine;

[CreateAssetMenu(fileName = "Item Data", menuName = "Scriptables/Item/Item")]
public class ItemData : ScriptableObject
{
    [SerializeField] LangText m_itemName;
    [SerializeField] LangText m_itemDesc;
    [SerializeField] Sprite m_itemIcon;
    public LangText itemName => m_itemName;
    public virtual LangText itemDesc => m_itemDesc;
    public Sprite itemIcon => m_itemIcon;
}
[System.Serializable]
public struct ItemIntPair
{
    public ItemData item;
    public int count;
}