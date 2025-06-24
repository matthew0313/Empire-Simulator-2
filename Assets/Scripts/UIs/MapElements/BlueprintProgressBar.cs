using System.Collections.Generic;
using UnityEngine;

public class BlueprintProgressBar : MonoBehaviour
{
    [SerializeField] Blueprint target;
    [SerializeField] GameObject bar;
    [SerializeField] Transform barFill;
    private void OnEnable()
    {
        target.onProgressChange += OnProgressChange;
        target.onIsPlacingChange += OnIsPlacingChange;
        OnProgressChange(); OnIsPlacingChange();
    }
    private void OnDisable()
    {
        target.onProgressChange -= OnProgressChange;
        target.onIsPlacingChange -= OnIsPlacingChange;
    }
    void OnIsPlacingChange()
    {
        bar.SetActive(!target.isPlacing);
    }
    void OnProgressChange()
    {
        barFill.localScale = new Vector2(target.progress / target.progressRequired, 1.0f);
    }
}