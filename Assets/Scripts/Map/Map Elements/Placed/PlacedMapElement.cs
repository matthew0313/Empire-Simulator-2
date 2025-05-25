using System.Collections.Generic;
using HexKit3D;
using UnityEngine;

public abstract class PlacedMapElement : MapElement
{
    [SerializeField] GameObject m_prefabOrigin;
    public GameObject prefabOrigin { get; private set; }
}
[System.Serializable]
public class PlacedMapElementSave
{
    public string id;
    public int prefabID;
    public Cubic position;
    public Quaternion rotation;
    public DataUnit data;
}