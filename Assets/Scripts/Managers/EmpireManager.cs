using System;
using System.Collections.Generic;
using UnityEngine;

public class EmpireManager : MonoBehaviour, ISavable
{
    public static EmpireManager Instance { get; private set; }
    public EmpireManager() => Instance = this;

    [SerializeField] List<ItemIntPair> startingItems = new();
    public readonly SerializableDictionary<ItemData, int> inventory = new();
    public Action<ItemData> onInventoryUpdate;
    public void AddItem(ItemData item, int count)
    {
        if (inventory.ContainsKey(item)) inventory[item] += count;
        else inventory[item] = count;
        onInventoryUpdate?.Invoke(item);
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
    readonly List<ItemData> searchAllList = new();
    public List<ItemData> SearchAll(Func<ItemData, bool> predicate)
    {
        searchAllList.Clear();
        foreach(var i in inventory.Keys)
        {
            int count = GetItemCount(i);
            if(predicate.Invoke(i) && count > 0)
            {
                searchAllList.Add(i);
            }
        }
        return searchAllList;
    }
    public bool RemoveItem(ItemData item, int count)
    {
        if (count <= 0) return true;
        if (GetItemCount(item) < count) return false;

        inventory[item] -= count;
        onInventoryUpdate?.Invoke(item);
        return true;
    }
    void Start()
    {
        if (!loaded)
        {
            foreach (var i in startingItems) AddItem(i.item, i.count);
        }
    }
    public void Save(SaveData data)
    {
        foreach (var i in inventory)
        {
            if (i.Key is ISavable savable) savable.Save(data);
            data.inventory.Add(i.Key, i.Value);
        }
    }
    bool loaded = false;
    public void Load(SaveData data)
    {
        loaded = true;
        foreach (var i in data.inventory)
        {
            if (i.Key is ISavable savable) savable.Load(data);
            inventory.Add(i.Key, i.Value);
        }
    }

    //Debug
    [SerializeField] ItemData addingItem;
    [SerializeField] int addingCount;
    public void Debug_AddItem()
    {
        if (!Application.isPlaying || addingItem == null || addingCount <= 0) return;
        AddItem(addingItem, addingCount);
    }
}

