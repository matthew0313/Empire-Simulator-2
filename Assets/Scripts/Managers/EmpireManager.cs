using System.Collections.Generic;
using UnityEngine;

public class EmpireManager : MonoBehaviour
{
    public static EmpireManager Instance { get; private set; }
    public EmpireManager() => Instance = this;
    public readonly SerializableDictionary<ItemData, int> inventory = new();
    public void AddItem(ItemData item, int count)
    {
        if (inventory.ContainsKey(item)) inventory[item] += count;
        else inventory[item] = count;
    }
    public int GetItemCount(ItemData item)
    {
        if (!inventory.ContainsKey(item)) return 0;
        else return inventory[item];
    }
    public bool RemoveItem(ItemData item, int count)
    {
        if (count <= 0) return true;
        if (GetItemCount(item) < count) return false;

        inventory[item] -= count;
        return true;
    }
}
