using System.Collections.Generic;
using HexKit3D;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GrassTile : MapTile
{
    [Header("GrassTile")]
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
#if UNITY_EDITOR
[CustomEditor(typeof(GrassTile))]
public class GrassTile_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Fix placedElement position"))
        {
            (target as MapTile).placedElement.transform.position = (target as MapTile).transform.position;
        }
    }
}
#endif
