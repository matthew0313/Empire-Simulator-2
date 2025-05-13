using System.Collections.Generic;
using UnityEngine;

namespace HexKit3D
{
    public class HexTilemap : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] float m_tileSize = 1.0f;
        [SerializeField] List<HexTile> m_placedTiles = new();
        public float tileSize => m_tileSize;
        public List<HexTile> placedTiles => m_placedTiles;

        [Header("Pathfinding")]
        [SerializeField] float maxHeightDifference = 1.1f;

        public readonly Dictionary<Cubic, HexTile> tiles = new();

        private void OnValidate()
        {
            foreach (var i in placedTiles)
            {
                i.transform.position = Cubic.CubicToPos(i.position, i.transform.position.y, tileSize);
            }
        }
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
        readonly List<HexTile> searchQueue = new(), searched = new();
        Cubic[] dir = new Cubic[6]
        {
        new(-1, 1, 0), new(-1, 0, 1), new(0, 1, -1), new(0, -1, 1), new(-1, 0, 1), new(-1, 1, 0)
        };
        public HexTilemapPath FindPath(Cubic startPosition, Cubic endPosition)
        {
            searchQueue.Clear(); searched.Clear();
            if (!tiles.TryGetValue(startPosition, out HexTile start)) return null;
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
                    if (tiles.TryGetValue(current.position + dir[i], out HexTile neighbor))
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