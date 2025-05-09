using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }
    public MapManager() => Instance = this;

    [Header("Grid")]
    [SerializeField] float m_cellX;
    [SerializeField] float m_cellZ;
    [SerializeField] LayerMask groundLayer, obstacleLayer;
    [SerializeField] float m_gridY = 0;
    public float cellX => m_cellX;
    public float cellZ => m_cellZ;
    public float gridY => m_gridY;

    [Header("Cell Visuals")]
    [SerializeField] CellVisuals cellVisualPrefab;
    [SerializeField] int renderDistance = 40;
    [SerializeField] int m_shrinkStartDist, m_maxDist;
    public int shrinkStartDist => m_shrinkStartDist;
    public int maxDist => m_maxDist;
    GameObject player;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        ResetCellVisuals();
    }
    private void Update()
    {
        UpdateCellVisuals();
    }
    CellVisuals[,] cellVisuals;
    Vector3Int lastPos;
    void ResetCellVisuals()
    {
        if(cellVisuals != null)
        {
            foreach (var i in cellVisuals) i.Release();
        }

        cellVisuals = new CellVisuals[renderDistance * 2 + 1, renderDistance * 2 + 1];
        Vector3Int pos = WorldToCell(player.transform.position);
        for (int i = 0; i < renderDistance * 2 + 1; i++)
        {
            for (int k = 0; k < renderDistance * 2 + 1; k++)
            {
                cellVisuals[i, k] = cellVisualPrefab.Get();
                cellVisuals[i, k].SetPos(pos + new Vector3Int(i - renderDistance, 0, k - renderDistance));
            }
        }
        lastPos = pos;
    }
    void UpdateCellVisuals()
    {
        Vector3Int pos = WorldToCell(player.transform.position);
        if(pos != lastPos)
        {
            for (int i = 0; i < renderDistance * 2 + 1; i++)
            {
                for (int k = 0; k < renderDistance * 2 + 1; k++)
                {
                    cellVisuals[i, k].SetPos(pos + new Vector3Int(i - renderDistance, 0, k - renderDistance));
                }
            }
            lastPos = pos;
        }
    }
    public bool TryGetCell(Vector3Int position, out CellData data)
    {
        if(Physics.Raycast(CellToWorld(position) + Vector3.up * 10.0f, Vector3.down, 20.0f, groundLayer))
        {
            data = new();
            data.obstructed = Physics.BoxCast(
                position, 
                new Vector3(cellX / 2.0f - 0.01f, 10.0f, cellZ / 2.0f - 0.01f), 
                Vector3.up, 
                Quaternion.identity, 
                0.0f, 
                obstacleLayer);
            return true;
        }
        data = new();
        return false;
    }
    public Vector3 CellToWorld(Vector3Int pos) => new Vector3(pos.x * cellX, gridY, pos.z * cellZ);
    public Vector3Int WorldToCell(Vector3 pos) => new Vector3Int(Mathf.FloorToInt(pos.x + 0.5f), 0, Mathf.FloorToInt(pos.z + 0.5f));
}
public struct CellData
{
    public bool obstructed;
}