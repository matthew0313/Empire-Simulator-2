using System.Collections.Generic;
using HexKit3D;
using UnityEngine;

public abstract class MapElement : MonoBehaviour
{
    public SaveID id;
    public virtual bool canPass => false;
    public MapTile placedTile { get; private set; }
    public void SetTile(MapTile tile)
    {
        placedTile = tile;
    }
    public virtual DataUnit Save() => new();
    public virtual void Load(DataUnit data) { }
}