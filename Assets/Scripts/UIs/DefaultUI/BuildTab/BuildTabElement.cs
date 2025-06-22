using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildTabElement : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] RawImage structureIcon;
    [SerializeField] TMP_Text structureName;
    Blueprint blueprint;
    BuildTab owner;
    public BuildTabElement Set(Blueprint blueprint, BuildTab owner)
    {
        this.blueprint = blueprint;
        this.owner = owner;
        structureIcon.texture = blueprint.structure.iconTexture;
        return this;
    }
    private void OnEnable()
    {
        Settings.onLanguageChange += OnLanguageChange;
        OnLanguageChange();

        button.onClick.AddListener(OnClick);
    }
    private void OnDisable()
    {
        Settings.onLanguageChange -= OnLanguageChange;
        button.onClick.RemoveListener(OnClick);
    }
    void OnLanguageChange()
    {
        structureName.text = blueprint.structure.structureName.text;
    }
    void OnClick()
    {
        owner.SelectBlueprint(blueprint);
    }
}