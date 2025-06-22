using System.Collections.Generic;
using UnityEngine;

namespace HexKit3D
{
    public class HexTile : MonoBehaviour
    {
        public HexTilemap owner;
        [HideInInspector] public Cubic position;
        public float height => transform.localPosition.y;

        //Pathfinding
        [HideInInspector] public float h, g;
        public float f => h + g;
        [HideInInspector] public HexTile prev;

        Cubic[] dir = new Cubic[6]
        {
            new(1, -1, 0), new(-1, 1, 0), new(0, 1, -1), new(0, -1, 1), new(1, 0, -1), new(-1, 0, 1)
        };
        public virtual bool isWalkable => true;
        public virtual IEnumerable<HexTile> GetNeighbors()
        {
            if (owner == null) yield break;
            for(int i = 0; i < 6; i++)
            {
                if (owner.TryGetTile(position + dir[i], out HexTile tile) && Mathf.Abs(tile.height - height) <= owner.maxHeightDifference) yield return tile;
            }
        }
        public virtual float GetDistance(HexTile other)
        {
            return Cubic.Distance(position, other.position);
        }
    }
    [System.Serializable]
    public struct Cubic
    {
        public Cubic(int q, int r, int s)
        {
            this.q = q;
            this.r = r;
            this.s = s;
        }
        public int q, r, s;
        public static bool operator ==(Cubic left, Cubic right)
        {
            return left.q == right.q && left.r == right.r && left.s == right.s;
        }
        public static bool operator !=(Cubic left, Cubic right)
        {
            return !(left == right);
        }
        public static Cubic operator +(Cubic left, Cubic right)
        {
            return new(left.q + right.q, left.r + right.r, left.s + right.s);
        }
        public static Cubic operator -(Cubic left, Cubic right)
        {
            return new(left.q - right.q, left.r - right.r, left.s - right.s);
        }
        public static Vector3 CubicToPos(Cubic hex, float height, float size)
        {
            return new Vector3(size * (3 / 2f * hex.q), height, size * (Mathf.Sqrt(3) / 2f * hex.q + Mathf.Sqrt(3) * hex.r));
        }
        public static Cubic PosToCubic(Vector3 pos, float size)
        {
            int q = Mathf.RoundToInt(2 / 3f * pos.x / size);
            int r = Mathf.RoundToInt((-1 / 3f * pos.x + Mathf.Sqrt(3) / 3 * pos.z) / size);
            return new Cubic(q, r, -q - r);
        }
        public static int Distance(Cubic a, Cubic b)
        {
            return (Mathf.Abs(a.q - b.q) + Mathf.Abs(a.r - b.r) + Mathf.Abs(a.s - b.s)) / 2;
        }
        public override bool Equals(object obj)
        {
            return obj is Cubic cubic && cubic == this;
        }
        public override int GetHashCode()
        {
            return (q, r, s).GetHashCode();
        }
        public override string ToString()
        {
            return $"({q}, {r}, {s})";
        }
        public static int Compare(Cubic a, Cubic b)
        {
            if (a.q > b.q) return 1;
            else if (a.q < b.q) return -1;
            else
            {
                if (a.r > b.r) return 1;
                else if (a.r < b.r) return -1;
                else return 0;
            }
        }
    }
}
#if UNITY_EDITOR
namespace HexKit3D.Editor
{
    using UnityEditor;
    [CustomEditor(typeof(HexTile))]
    public class HexTile_Editor : Editor
    {
        
    }
}
#endif