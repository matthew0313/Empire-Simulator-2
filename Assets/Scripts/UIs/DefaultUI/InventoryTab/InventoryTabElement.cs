using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryTabElement : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Image itemIcon;
    [SerializeField] TMP_Text itemCount;

    ItemData item;
    InventoryTab owner;
    public InventoryTabElement Set(ItemData item, InventoryTab owner)
    {
        this.item = item;
        this.owner = owner;
        itemIcon.sprite = item.itemIcon;
        button.onClick.AddListener(OnClick);
        return this;
    }
    private void OnEnable()
    {
        EmpireManager.Instance.onInventoryUpdate += OnInventoryUpdate;
        OnInventoryUpdate(item);
    }
    private void OnDisable()
    {
        EmpireManager.Instance.onInventoryUpdate -= OnInventoryUpdate;
    }
    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClick);
    }
    void OnInventoryUpdate(ItemData updated)
    {
        if(updated == item)
        {
            itemCount.text = $"x{EmpireManager.Instance.GetItemCount(item)}";
        }
    }
    void OnClick()
    {
        owner.DisplayItem(item);
    }
}