using System.Collections.Generic;
using HexKit3D;
using UnityEngine;

public abstract class FixedMapElement : MapElement
{
    [Header("FIxed Map Element")]
    [SerializeField] SaveID m_id;
    public SaveID id => m_id;
}