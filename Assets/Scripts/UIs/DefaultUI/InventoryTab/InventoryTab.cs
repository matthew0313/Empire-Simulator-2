using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InventoryTab : Tab
{
    [Header("Tween")]
    [SerializeField] RectTransform tweenTarget;
    [SerializeField] float tweenTime = 0.5f;
    [SerializeField] Ease tweenEase = Ease.OutCirc;

    [Header("Elements")]
    [SerializeField] InventoryTabElement elementPrefab;
    [SerializeField] Transform elementAnchor;

    [Header("Item Display")]
    [SerializeField] Image itemImage;
    [SerializeField] TMP_Text itemName, itemCategory, itemDesc;
    readonly Dictionary<ItemData, InventoryTabElement> elements = new();

    Tween tween;
    float tweenCounter = 0.0f;
    public override void Open()
    {
        base.Open();
        gameObject.SetActive(true);
        if (tween == null) tween = DOTween.To(() => tweenCounter, value => tweenCounter = value, 1.0f, tweenTime)
                .SetAutoKill(false)
                .SetEase(tweenEase)
                .OnUpdate(() =>
                {
                    tweenTarget.anchorMin = new Vector2(0.0f, 1.0f - tweenCounter);
                    tweenTarget.anchorMax = new Vector2(1.0f, 2.0f - tweenCounter);
                });
        tween.OnStepComplete(null);
        tween.PlayForward();
    }
    public override void Close()
    {
        base.Close();
        tween.OnStepComplete(() => gameObject.SetActive(false));
        tween.PlayBackwards();
    }

    int currentCategory = 0;
    ItemData displaying = null;
    private void OnEnable()
    {
        EmpireManager.Instance.onInventoryUpdate += OnInventoryUpdate;
        foreach (var i in EmpireManager.Instance.inventory.Keys) OnInventoryUpdate(i);

        Settings.onLanguageChange += OnLanguageChange;
        OnLanguageChange();
    }
    private void OnDisable()
    {
        EmpireManager.Instance.onInventoryUpdate -= OnInventoryUpdate;
        Settings.onLanguageChange -= OnLanguageChange;
    }
    void OnInventoryUpdate(ItemData item)
    {
        if (!elements.ContainsKey(item))
        {
            var tmp = Instantiate(elementPrefab, elementAnchor).Set(item, this);
            tmp.gameObject.SetActive(true);
            elements.Add(item, tmp);
            ChangeCategory(currentCategory);
        }
    }
    void OnLanguageChange()
    {
        DisplayItem(displaying);
    }
    public void ChangeCategory(int category)
    {
        currentCategory = category;
        foreach(var i in elements)
        {
            if (category <= 0 || ((int)i.Key.category & 1 << (category - 1)) > 0) i.Value.gameObject.SetActive(true);
            else i.Value.gameObject.SetActive(false);
        }
    }
    public void DisplayItem(ItemData item)
    {
        displaying = item;
        if(item == null)
        {
            itemImage.enabled = false;
            itemName.text = new LangText()
            {
                en = "No item selected",
                kr = "선택된 아이템 없음"
            }.text;
            itemCategory.text = new LangText()
            {
                en = "Category",
                kr = "카테고리"
            }.text;
            itemDesc.text = new LangText()
            {
                en = "Description",
                kr = "설명"
            }.text;
        }
        else
        {
            itemImage.enabled = true;
            itemImage.sprite = item.itemIcon;
            itemName.text = item.itemName.text;
            itemCategory.text = "";
            bool prev = false;
            foreach(ItemCategory i in Enum.GetValues(typeof(ItemCategory)))
            {
                if((i & item.category) > 0)
                {
                    if (prev) itemCategory.text += ", ";
                    prev = true;
                    itemCategory.text += ItemData.CategoryToText(i).text;
                }
            }
            itemDesc.text = item.itemDesc.text;
        }
    }
}