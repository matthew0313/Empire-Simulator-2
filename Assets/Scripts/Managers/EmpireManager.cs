using System;
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
    public bool Search(Func<ItemData, bool> predicate, out ItemIntPair results)
    {
        results = new();
        foreach(var i in inventory.Keys)
        {
            results.count = GetItemCount(i);
            if(predicate.Invoke(i) && results.count > 0)
            {
                results.item = i;
                return true;
            }
        }
        return false;
    }
    public IEnumerable<ItemIntPair> SearchAll(Func<ItemData, bool> predicate)
    {
        foreach(var i in inventory.Keys)
        {
            int count = GetItemCount(i);
            if(predicate.Invoke(i) && count > 0)
            {
                yield return new() { item = i, count = count };
            }
        }
    }
    readonly List<ItemIntPair> searchAllList = new();
    public IEnumerable<ItemIntPair> SearchAll(Func<ItemData, bool> predicate, Func<ItemData, ItemData, int> comparison)
    {
        searchAllList.Clear();
        foreach (var i in SearchAll(predicate)) searchAllList.Add(i);
        searchAllList.Sort((a, b) => comparison.Invoke(a.item, b.item));
        foreach (var i in searchAllList) yield return i;
    }
    public bool RemoveItem(ItemData item, int count)
    {
        if (count <= 0) return true;
        if (GetItemCount(item) < count) return false;

        inventory[item] -= count;
        return true;
    }
}
