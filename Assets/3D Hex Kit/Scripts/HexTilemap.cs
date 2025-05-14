using System.Collections.Generic;
using UnityEngine;

namespace HexKit3D
{
    public class HexTilemap : MonoBehaviour
    {
        [Header("Grid")]
        [SerializeField] List<HexTile> m_placedTiles = new();
        public List<HexTile> placedTiles => m_placedTiles;

        [Header("Settings")]
        [SerializeField] HexTilemapSettings settings;
        public float tileSize => settings.tileSize;
        public float maxHeightDifference => settings.maxHeightDifference;

        public readonly Dictionary<Cubic, HexTile> tiles = new();
        private void Awake()
        {
            foreach (var i in placedTiles)
            {
                if (tiles.ContainsKey(i.position))
                {
                    Destroy(i.gameObject); continue;
                }
                tiles.Add(i.position, i);
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(new Vector3(transform.position.x, 10000.0f, transform.position.z), new Vector3(transform.position.x, -10000.0f, transform.position.z));
        }
        readonly List<HexTile> searchQueue = new(), searched = new();
        Cubic[] dir = new Cubic[6]
        {
            new(-1, 1, 0), new(-1, 0, 1), new(0, 1, -1), new(0, -1, 1), new(-1, 0, 1), new(-1, 1, 0)
        };
        public bool TryGetTile(Cubic cubic, out HexTile tile)
        {
            return tiles.TryGetValue(cubic, out tile);
        }
        public HexTilemapPath FindPath(Cubic startPosition, Cubic endPosition)
        {
            searchQueue.Clear(); searched.Clear();
            if (!TryGetTile(startPosition, out HexTile start)) return null;
            start.g = 0;
            start.h = Cubic.Distance(start.position, endPosition);
            searchQueue.Add(start);

            while (searchQueue.Count > 0)
            {
                searchQueue.Sort((a, b) =>
                {
                    if (a.f < b.f) return -1;
                    else if (a.f > b.f) return 1;
                    else
                    {
                        if (a.h < b.h) return -1;
                        else if (a.h > b.h) return 1;
                        else return 0;
                    }
                });
                HexTile current = searchQueue[0];
                searchQueue.RemoveAt(0);
                for (int i = 0; i < 6; i++)
                {
                    if (TryGetTile(current.position + dir[i], out HexTile neighbor))
                    {
                        if (IsMovable(current, neighbor))
                        {
                            if (neighbor.position == endPosition)
                            {
                                neighbor.prev = current;
                                HexTile tmp = neighbor;
                                HexTilemapPath path = new();
                                while (tmp.prev != null)
                                {
                                    path.route.Insert(0, tmp);
                                }
                                return path;
                            }
                            else if (!searched.Contains(neighbor) && (!searchQueue.Contains(neighbor) || neighbor.g > current.g + 1))
                            {
                                neighbor.prev = current;
                                neighbor.g = current.g + 1;
                                neighbor.h = Cubic.Distance(neighbor.position, endPosition);
                                searchQueue.Add(neighbor);
                            }
                        }
                    }
                }
            }
            return null;
        }
        protected virtual bool IsMovable(HexTile start, HexTile to)
        {
            return Mathf.Abs(start.height - to.height) <= maxHeightDifference && start.IsMovable(to);
        }
    }
    public class HexTilemapPath
    {
        public readonly List<HexTile> route = new();
    }
}
#if UNITY_EDITOR
namespace HexKit3D.Editor
{
    using UnityEditor;
    [CustomEditor(typeof(HexTilemap))]
    public class HexTilemap_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(GUILayout.Button("Fix Tile Position"))
            {
                foreach (var i in ((HexTilemap)target).placedTiles)
                {
                    i.transform.position = Cubic.CubicToPos(i.position, i.transform.position.y, ((HexTilemap)target).tileSize);
                }
            }
        }
    }
}
#endif