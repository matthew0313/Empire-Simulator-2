using System.Collections.Generic;
using HexKit3D;
using UnityEngine;

public class SeasonalMaterialChanger : MonoBehaviour
{
    [SerializeField] MeshRenderer rend;
    [SerializeField] Material spring, summer, fall, winter;
    private void OnEnable()
    {
        GameManager.Instance.onDayChange += OnDayChange;
        OnDayChange();
    }
    private void OnDisable()
    {
        GameManager.Instance.onDayChange -= OnDayChange;
    }
    void OnDayChange()
    {
        switch (GameManager.Instance.currentSeason)
        {
            case Season.Spring: rend.material = spring; break;
            case Season.Summer: rend.material = summer; break;
            case Season.Fall: rend.material = fall; break;
            case Season.Winter: rend.material = winter; break;
        }
    }
}