using UnityEngine;

namespace HexKit3D
{
    public class HexTile : MonoBehaviour
    {
        public HexTilemap owner;
        public Cubic position;
        public float height => transform.localPosition.y;

        //Pathfinding
        [HideInInspector] public int h, g;
        public int f => h + g;
        [HideInInspector] public HexTile prev;
        public virtual bool IsMovable(HexTile other) => true;
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
    }
}