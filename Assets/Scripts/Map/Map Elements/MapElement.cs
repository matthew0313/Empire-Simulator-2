using System.Collections.Generic;
using HexKit3D;
using UnityEngine;

public abstract class MapElement : MonoBehaviour, IMapElement
{
    public MapElement self => this;

    public Island placedIsland;
    public Cubic position => placedIsland.tilemap.PosToCubic(transform.position);
    public virtual DataUnit Save() => new();
    public virtual void Load(DataUnit data) { }
}