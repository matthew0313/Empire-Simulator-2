using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FarmProgressBar : MonoBehaviour
{
    [SerializeField] Farm target;
    [SerializeField] Image cropImage;
    [SerializeField] Transform barFill;
    private void OnEnable()
    {
        target.onGrowthChange += OnGrowthChange;
        OnGrowthChange();

        cropImage.sprite = target.crop.itemIcon;
    }
    private void OnDisable()
    {
        target.onGrowthChange -= OnGrowthChange;
    }
    void OnGrowthChange()
    {
        barFill.localScale = new Vector2(target.growth / target.growthRequired, 1.0f);
    }
}