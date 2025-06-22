using System.Collections.Generic;
using HexKit3D;
using UnityEngine;

public abstract class Structure : PlacedMapElement
{
    [Header("Structure")]
    [SerializeField] LangText m_structureName;
    [SerializeField] LangText m_structureDesc;
    [SerializeField] ItemIntPair[] m_buildIngredients;
    [SerializeField] int m_buildLevelRequirement = 1;
    [SerializeField] int m_buildMaxWorkers = 3;
    [SerializeField] float m_buildProgressRequired = 100.0f;
    public LangText structureName => m_structureName;
    public virtual LangText structureDesc => m_structureDesc;
    public ItemIntPair[] buildIngredients => m_buildIngredients;
    public int buildLevelRequirement => m_buildLevelRequirement;
    public int buildMaxWorkers => m_buildMaxWorkers;
    public float buildProgressRequired => m_buildProgressRequired;

    const float cameraDistance = 2.0f;
    const float cameraHeight = 0.5f, cameraAngle = 10.0f;
    public virtual StructureCategory category { get; } = 0;
    RenderSpace renderSpace;
    public RenderTexture iconTexture
    {
        get
        {
            if(renderSpace == null)
            {
                renderSpace = RenderSpace.Create().SetObject(Instantiate(this).gameObject).SetCamera(cameraDistance, cameraHeight, cameraAngle);
            }
            return renderSpace.texture;
        }
    }
    public static LangText CategoryToText(StructureCategory category)
    {
        switch (category)
        {
            case StructureCategory.Residental: return new()
            {
                en = "Residental",
                kr = "거주지"
            };
            case StructureCategory.Workplace: return new()
            {
                en = "Workplace",
                kr = "직업소"
            };
            case StructureCategory.Agriculture: return new()
            {
                en = "Agriculture",
                kr = "농사"
            };
        }
        return new();
    }
}
[System.Serializable]
[System.Flags]
public enum StructureCategory
{
    Residental = 1<<0,
    Workplace = 1<<1,
    Agriculture = 1<<2
}