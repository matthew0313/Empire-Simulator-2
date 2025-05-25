using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveData
{
    public SerializableDictionary<ItemData, int> inventory = new();
    public SerializableDictionary<string, IslandSave> islandSaves = new();
}