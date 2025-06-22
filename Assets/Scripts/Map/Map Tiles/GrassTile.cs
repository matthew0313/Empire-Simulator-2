using System.Collections.Generic;
using HexKit3D;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GrassTile : MapTile
{
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

    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(GrassTile))]
public class GrassTile_Editor : MapTile_Editor
{

}
#endif
