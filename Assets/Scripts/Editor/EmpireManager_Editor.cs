using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.Burst.Intrinsics.X86.Avx;

[CustomEditor(typeof(EmpireManager))]
public class EmpireManager_Editor : Editor
{
    public VisualTreeAsset visualTreeAsset;
    public VisualTreeAsset InventoryContents_Element;
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement tree = visualTreeAsset.CloneTree();

        tree.Q<Button>("AddItemButton").clicked += (target as EmpireManager).Debug_AddItem;

        VisualElement inventoryContents = tree.Q<VisualElement>("InventoryContents");
        Dictionary<ItemData, VisualElement> inventoryContentsItemList = new();

        if (Application.isPlaying)
        {
            Action<ItemData> onInventoryUpdate = item =>
            {
                if (!inventoryContentsItemList.ContainsKey(item))
                {
                    VisualElement tmp = InventoryContents_Element.CloneTree();
                    tmp.Q<VisualElement>("ItemImage").style.backgroundImage = new() { value = new() { sprite = item.itemIcon } };
                    tmp.Q<Label>("ItemName").text = item.itemName.en;
                    inventoryContents.Add(tmp);
                    inventoryContentsItemList[item] = tmp;
                }
                VisualElement tree = inventoryContentsItemList[item];
                tree.Q<Label>("ItemCount").text = (target as EmpireManager).GetItemCount(item).ToString();
            };

            foreach (var i in (target as EmpireManager).inventory.Keys) onInventoryUpdate.Invoke(i);
            (target as EmpireManager).onInventoryUpdate += onInventoryUpdate;
        }

        return tree;
    }
}
