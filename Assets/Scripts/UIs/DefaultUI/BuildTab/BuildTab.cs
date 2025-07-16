using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BuildTab : Tab
{
    [Header("Tween")]
    [SerializeField] RectTransform tweenTarget;
    [SerializeField] float tweenTime = 0.5f;
    [SerializeField] Ease tweenEase = Ease.OutCirc;

    [Header("Elements")]
    [SerializeField] BuildTabElement elementPrefab;
    [SerializeField] Transform elementAnchor;
    readonly List<BuildTabElement> elements = new();

    [Header("Structure Display")]
    [SerializeField] RawImage structureImage;
    [SerializeField] TMP_Text structureName, structureCategory, structureDesc;
    [SerializeField] BuildTabIngredientElement ingredientElementPrefab;
    [SerializeField] Transform ingredientElementAnchor;
    [SerializeField] Button buildButton;
    readonly List<BuildTabIngredientElement> ingredientElements = new();
    bool initialized = false;
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
    void Init()
    {
        initialized = true;
        GameManager.Instance.onBlueprintUnlock += OnBlueprintUnlock;
        foreach (var i in GameManager.Instance.unlockedBlueprints) OnBlueprintUnlock(i);
    }
    private void OnDestroy()
    {
        GameManager.Instance.onBlueprintUnlock -= OnBlueprintUnlock;
    }
    private void OnEnable()
    {
        if (!initialized) Init();
        Settings.onLanguageChange += OnLanguageChange;
        OnLanguageChange();

        buildButton.onClick.AddListener(OnBuildButtonClick);
    }
    private void OnDisable()
    {
        Settings.onLanguageChange -= OnLanguageChange;
        buildButton.onClick.RemoveListener(OnBuildButtonClick);
    }
    void OnBlueprintUnlock(Blueprint blueprint)
    {
        var tmp = Instantiate(elementPrefab, elementAnchor).Set(blueprint, this);
        tmp.gameObject.SetActive(true);
        elements.Add(tmp);
    }
    Blueprint selected = null;
    void OnLanguageChange()
    {
        SelectBlueprint(selected);
    }
    public void SelectBlueprint(Blueprint blueprint)
    {
        selected = blueprint;
        if(blueprint == null)
        {
            structureImage.enabled = false;
            structureName.text = new LangText()
            {
                en = "No structure selected",
                kr = "선택된 건물 없음"
            }.text;
            structureCategory.text = new LangText()
            {
                en = "Category",
                kr = "카테고리"
            }.text;
            structureDesc.text = new LangText()
            {
                en = "Description",
                kr = "설명"
            }.text;
            ingredientElementAnchor.gameObject.SetActive(false);
            buildButton.gameObject.SetActive(false);
        }
        else
        {
            Structure structure = blueprint.structure;
            structureImage.enabled = true;
            structureImage.texture = structure.iconTexture;
            structureName.text = structure.structureName.text;
            structureCategory.text = "";
            bool prev = false;
            foreach(StructureCategory i in Enum.GetValues(typeof(StructureCategory)))
            {
                if((structure.category & i) > 0)
                {
                    if (prev) structureCategory.text += ", ";
                    prev = true;
                    structureCategory.text += Structure.CategoryToText(i).text;
                }
            }
            structureDesc.text = structure.structureDesc.text;
            int k = 0;
            for(k = 0; k < structure.buildIngredients.Length; k++)
            {
                if (ingredientElements.Count <= k) ingredientElements.Add(Instantiate(ingredientElementPrefab, ingredientElementAnchor));
                ingredientElements[k].gameObject.SetActive(true);
                ingredientElements[k].Set(structure.buildIngredients[k].item, structure.buildIngredients[k].count);
            }
            for(; k < ingredientElements.Count; k++)
            {
                ingredientElements[k].gameObject.SetActive(false);
            }
            ingredientElementAnchor.gameObject.SetActive(true);
            buildButton.gameObject.SetActive(true);
        }
    }
    void OnBuildButtonClick()
    {
        if (selected == null) return;
        GameManager.Instance.EnterBuildMode(selected);
    }
}