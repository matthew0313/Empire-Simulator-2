using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe Data", menuName = "Scriptables/Recipe Data")]
public class RecipeData : ScriptableObject, ISavable
{
    [SerializeField] ItemIntPair[] m_ingredients;
    [SerializeField] LootTableElement[] results;
    [SerializeField] float m_progressRequired = 10.0f;
    public ItemIntPair[] ingredients => m_ingredients;
    public float progressRequired => m_progressRequired;

    public bool unlocked = true;
    public int targetAmount = 100;
    public int priority = 0;

    public ItemData primaryResult => results[0].item;
    public IEnumerable<ItemIntPair> GetResults()
    {
        foreach (var i in results)
        {
            float chance = i.chance;
            int count = Mathf.FloorToInt(chance / 100.0f);
            chance -= count * 100.0f;
            if (UnityEngine.Random.Range(0.0f, 100.0f) <= chance)
            {
                count++;
            }
            yield return new() { item = i.item, count = count };
        }
    }
    public void Save(SaveData data)
    {
        DataUnit tmp = new();
        tmp.bools["unlocked"] = unlocked;
        tmp.integers["targetAmount"] = targetAmount;
        tmp.integers["priority"] = priority;
        data.recipeSettings[this] = tmp;
    }
    public void Load(SaveData data)
    {
        if(data.recipeSettings.TryGetValue(this, out DataUnit tmp))
        {
            unlocked = tmp.bools["unlocked"];
            targetAmount = tmp.integers["targetAmount"];
            priority = tmp.integers["priority"];
        }
    }
}