using System.Collections.Generic;
using HexKit3D;
using UnityEngine;

public class MapTile : HexTile
{
    public MapElement placedElement;
    public override bool isWalkable => placedElement == null || placedElement.canPass;
    private void Awake()
    {
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