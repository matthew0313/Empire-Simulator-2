using HexKit3D;
using UnityEngine;

public class Island : MonoBehaviour
{
    [SerializeField] HexTilemap m_hex;
    [SerializeField] Vector3 moveBoundsCenter, m_moveBoundsMin, m_moveBoundsMax;
    public HexTilemap hex => m_hex;
    public Vector3 moveBoundsMin => transform.position + moveBoundsCenter + m_moveBoundsMin;
    public Vector3 moveBoundsMax => transform.position + moveBoundsCenter + m_moveBoundsMax;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube((moveBoundsMax + moveBoundsMin) / 2.0f, moveBoundsMax - moveBoundsMin);
    }
}