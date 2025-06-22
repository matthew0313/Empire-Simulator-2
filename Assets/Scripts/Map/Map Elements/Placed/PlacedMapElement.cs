using System.Collections.Generic;
using HexKit3D;
using UnityEngine;

public abstract class PlacedMapElement : MapElement
{
    [Header("Placed map element")]
    [SerializeField] string m_id;
    public string id => m_id;
}
[System.Serializable]
public class PlacedMapElementSave
{
    public string id;
    public Vector3 position;
    public Quaternion rotation;
    public DataUnit data;
}