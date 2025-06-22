using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Loot Table", menuName = "Scriptables/Loot Table")]
public class RecipeData : ScriptableObject, ISavable
{
    [SerializeField] ItemIntPair[] m_ingredients, m_results;
    public ItemIntPair[] ingredients => m_ingredients;
    public ItemIntPair[] results => m_results;
    public int targetAmount = 100;
    public int priority = 0;

    public void Save(SaveData data)
    {
        DataUnit tmp = new();
        tmp.integers["targetAmount"] = targetAmount;
        tmp.integers["priority"] = priority;
        data.recipeSettings[this] = tmp;
    }
    public void Load(SaveData data)
    {
        if(data.recipeSettings.TryGetValue(this, out DataUnit tmp))
        {
            targetAmount = tmp.integers["targetAmount"];
            priority = tmp.integers["priority"];
        }
    }
}