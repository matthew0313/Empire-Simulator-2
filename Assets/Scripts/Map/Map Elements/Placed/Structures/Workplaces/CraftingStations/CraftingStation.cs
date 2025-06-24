using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using HexKit3D;
using UnityEngine;
using UnityEngine.UI;

public class CraftingStation : Structure, IWorkplace
{
    [Header("Crafting Station")]
    [SerializeField] int m_levelRequirement = 0;
    [SerializeField] int m_maxWorkers = 3;
    [SerializeField] RecipeData[] m_recipes;
    public int levelRequirement => m_levelRequirement;
    public int maxWorkers => m_maxWorkers;
    public RecipeData[] recipes => m_recipes;
    public List<NPC> workers { get; } = new();
    public RecipeData selectedRecipe { get; private set; } = null;
    public Action onSelectedRecipeChange;
    public float progress { get; private set; } = 0.0f;
    public Action onProgressChange;

    const float searchRate = 1.0f;
    float counter = 0.0f;
    private void Update()
    {
        if (selectedRecipe != null) return;
        counter += Time.deltaTime;
        if(counter >= searchRate)
        {
            SearchRecipe();
        }
    }
    readonly List<RecipeData> searchRecipeList = new();
    void SearchRecipe()
    {
        counter = 0.0f;
        searchRecipeList.Clear();
        foreach(var i in recipes)
        {
            if (EmpireManager.Instance.GetItemCount(i.primaryResult) >= i.targetAmount) continue;

            foreach(var k in i.ingredients)
            {
                if (EmpireManager.Instance.GetItemCount(k.item) < k.count) continue;
            }

            searchRecipeList.Add(i);
        }
        searchRecipeList.Sort((a, b) => b.priority.CompareTo(a.priority));
        if (searchRecipeList.Count > 0)
        {
            foreach (var i in searchRecipeList[0].ingredients)
            {
                EmpireManager.Instance.RemoveItem(i.item, i.count);
            }
            selectedRecipe = searchRecipeList[0];
        }
        onSelectedRecipeChange?.Invoke();
    }
    public void AddProgress(float amount)
    {
        if (selectedRecipe == null) return;

        progress += amount;
        if (progress >= selectedRecipe.progressRequired)
        {
            progress = 0.0f;
            foreach (var i in selectedRecipe.GetResults())
            {
                EmpireManager.Instance.AddItem(i.item, i.count);
            }
            selectedRecipe = null;
            SearchRecipe();
        }
        onProgressChange?.Invoke();
    }
}