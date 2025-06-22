using System.Collections.Generic;
using HexKit3D;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapTile : HexTile
{
    public Island island { get; private set; }
    public override bool isWalkable
    {
        get
        {
            return !Physics.Raycast(transform.position, Vector3.up, 1000.0f, GameManager.Instance.obstructionLayer);
        }
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(MapTile))]
public class MapTile_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Check Walkability")) Debug.Log((target as MapTile).isWalkable);
    }
}
#endif