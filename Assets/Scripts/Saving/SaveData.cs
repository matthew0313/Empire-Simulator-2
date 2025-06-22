using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SaveData
{
    //inventory
    public SerializableDictionary<ItemData, int> inventory = new();
    public SerializableDictionary<ItemData, DataUnit> itemSettings = new();
    public SerializableDictionary<RecipeData, DataUnit> recipeSettings = new();
    public SerializableDictionary<string, IslandSave> islandSaves = new();
    public List<string> unlockedBlueprints = new();
}