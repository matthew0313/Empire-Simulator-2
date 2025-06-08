using System.Collections.Generic;
using HexKit3D;
using UnityEngine;

public abstract class MapElement : MonoBehaviour, IMapElement
{
    public MapElement self => this;
    public SaveID id;
    public virtual bool canPass => true;
    public MapTile placedTile { get; private set; }
    public Island placedIsland => placedTile.island;
    public Cubic position => placedTile.position;
    public void SetTile(MapTile tile)
    {
        placedTile = tile;
    }
    public virtual DataUnit Save() => new();
    public virtual void Load(DataUnit data) { }
}