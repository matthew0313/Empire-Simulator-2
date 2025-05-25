using System;
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

        public readonly List<(Cubic, HexTile)> tiles = new();
        private void Awake()
        {
            foreach (var i in placedTiles)
            {
                tiles.Add((i.position, i));
            }
            tiles.Sort((a, b) => Cubic.Compare(a.Item1, b.Item1));
        }
        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(new Vector3(transform.position.x, 10000.0f, transform.position.z), new Vector3(transform.position.x, -10000.0f, transform.position.z));
        }
        readonly List<HexTile> searchQueue = new(), searched = new();
        public bool TryGetTile<T>(Cubic cubic, out T tile) where T : HexTile
        {
            int tmp = tiles.LowerBound((a, b) => Cubic.Compare(a.Item1, b.Item1), (cubic, null));
            if(tmp != -1 && tiles[tmp].Item2 is T)
            {
                tile = tiles[tmp].Item2 as T;
                return true;
            }
            tile = null;
            return false;
        }
        public HexTilemapPath<T> FindPath<T>(Cubic startPosition, Cubic endPosition) where T : HexTile
        {
            searchQueue.Clear(); searched.Clear();
            if (!TryGetTile(startPosition, out T start) || !TryGetTile(endPosition, out T end)) return null;
            start.g = 0;
            start.h = Cubic.Distance(start.position, endPosition);
            start.prev = null;
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
                T current = searchQueue[0] as T;
                searchQueue.RemoveAt(0);
                if (current.position == endPosition)
                {
                    T tmp = current;
                    HexTilemapPath<T> path = new() { tilemap = this };
                    while (tmp != null)
                    {
                        path.route.Insert(0, tmp);
                        tmp = tmp.prev as T;
                    }
                    return path;
                }
                foreach (var i in current.GetNeighbors())
                {
                    T neighbor = i as T;
                    if (neighbor == null) continue;

                    if (!neighbor.isWalkable)
                    {
                        if (neighbor.position == endPosition)
                        {
                            T tmp = neighbor;
                            tmp.prev = current;
                            HexTilemapPath<T> path = new() { tilemap = this };
                            while (tmp != null)
                            {
                                path.route.Insert(0, tmp);
                                tmp = tmp.prev as T;
                            }
                            return path;
                        }
                        continue;
                    }
                    if (!searched.Contains(neighbor) && !searchQueue.Contains(neighbor))
                    {
                        neighbor.prev = current;
                        neighbor.g = current.g + current.GetDistance(neighbor);
                        neighbor.h = current.GetDistance(neighbor);
                        searchQueue.Add(neighbor);
                    }
                    else if (searchQueue.Contains(neighbor) && neighbor.g > current.g + current.GetDistance(neighbor))
                    {
                        neighbor.prev = current;
                        neighbor.g = current.g + current.GetDistance(neighbor);
                    }
                }
                searched.Add(current);
            }
            return null;
        }
        public Cubic PosToCubic(Vector3 pos)
        {
            return Cubic.PosToCubic(transform.InverseTransformPoint(pos), tileSize);
        }
        public Vector3 CubicToPos(Cubic cubic)
        {
            return transform.TransformPoint(Cubic.CubicToPos(cubic, 0.0f, tileSize));
        }
    }
    public class HexTilemapPath<T>
    {
        public HexTilemap tilemap;
        public readonly List<T> route = new();
    }
    public static class ListUtility
    {
        public static int LowerBound<T>(this List<T> list, Func<T, T, int> compare, T item)
        {
            int start = 0, end = list.Count;
            int current = 0;
            while(start < end)
            {
                current = (start + end) / 2;
                int tmp = compare.Invoke(list[current], item);
                if (tmp > 0)
                {
                    end = current;
                }
                else if(tmp < 0)
                {
                    start = current + 1;
                }
                else
                {
                    if(current <= 0 || compare.Invoke(list[current - 1], item) < 0)
                    {
                        return current;
                    }
                    else
                    {
                        end = current;
                    }
                }
            }
            return -1;
        }
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