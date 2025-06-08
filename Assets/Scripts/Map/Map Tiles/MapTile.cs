using System.Collections.Generic;
using HexKit3D;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapTile : HexTile
{
    public Island island { get; private set; }
    public MapElement placedElement;
    public override bool isWalkable => placedElement == null || placedElement.canPass;
    public void Set(Island island)
    {
        this.island = island;
        if (placedElement != null) placedElement.SetTile(this);
    }
    public override IEnumerable<HexTile> GetNeighbors()
    {
        foreach(var i in base.GetNeighbors())
        {
            if (i is MapTile tile) yield return i;
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
        if(GUILayout.Button("Fix placedElement position"))
        {
            (target as MapTile).placedElement.transform.position = (target as MapTile).transform.position;
        }
    }
}
#endif
