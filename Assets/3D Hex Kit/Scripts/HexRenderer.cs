using System.Collections.Generic;
using UnityEngine;

namespace HexKit3D
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class HexRenderer : MonoBehaviour
    {
        [SerializeField] float m_innerRadius, m_outerRadius = 1.0f, m_height = 1.0f;
        public float innerRadius
        {
            get => m_innerRadius;
            set
            {
                m_innerRadius = value;
                DrawMesh();
            }
        }
        public float outerRadius
        {
            get => m_outerRadius;
            set
            {
                m_outerRadius = value;
                DrawMesh();
            }
        }
        public float height
        {
            get => m_height;
            set
            {
                m_height = value;
                DrawMesh();
            }
        }

        Mesh m_mesh;
        Mesh mesh
        {
            get
            {
                if(m_mesh == null)
                {
                    m_mesh = new Mesh();
                    m_mesh.name = "Hex";
                    meshFilter.mesh = m_mesh;
                }
                return m_mesh;
            }
        }
        MeshRenderer m_meshRenderer;
        public MeshRenderer meshRenderer
        {
            get
            {
                if (m_meshRenderer == null) m_meshRenderer = GetComponent<MeshRenderer>();
                return m_meshRenderer;
            }
        }
        MeshFilter m_meshFilter;
        public MeshFilter meshFilter
        {
            get
            {
                if(m_meshFilter == null) m_meshFilter = GetComponent<MeshFilter>();
                return m_meshFilter;
            }
        }
        private void OnValidate()
        {
            DrawMesh();
        }
        private void OnEnable()
        {
            DrawMesh();
        }
        void DrawMesh()
        {
            DrawFaces();
            CombineFaces();
        }
        readonly List<Face> faces = new();
        void DrawFaces()
        {
            faces.Clear();
            for (int i = 0; i < 6; i++)
            {
                faces.Add(CreateFace(innerRadius, outerRadius, height / 2.0f, height / 2.0f, i, false));
            }
            for (int i = 0; i < 6; i++)
            {
                faces.Add(CreateFace(innerRadius, outerRadius, -height / 2.0f, -height / 2.0f, i, true));
            }
            for (int i = 0; i < 6; i++)
            {
                faces.Add(CreateFace(outerRadius, outerRadius, height / 2.0f, -height / 2.0f, i, true));
            }
            for (int i = 0; i < 6; i++)
            {
                faces.Add(CreateFace(innerRadius, innerRadius, height / 2.0f, -height / 2.0f, i, false));
            }
        }
        Face CreateFace(float innerRadius, float outerRadius, float heightA, float heightB, int point, bool reverse = false)
        {
            Vector3 p1 = GetPoint(innerRadius, heightB, point);
            Vector3 p2 = GetPoint(innerRadius, heightB, point + 1);
            Vector3 p3 = GetPoint(outerRadius, heightA, point + 1);
            Vector3 p4 = GetPoint(outerRadius, heightA, point);

            List<Vector3> vertices = new() { p1, p2, p3, p4 };
            List<int> triangles = new() { 0, 1, 2, 2, 3, 0 };
            List<Vector2> uvs = new() { new(0, 0), new(1, 0), new(1, 1), new(0, 1) };
            if (reverse) vertices.Reverse();

            return new Face(vertices, triangles, uvs);
        }
        Vector3 GetPoint(float size, float height, int index)
        {
            float angle = 60 * index * Mathf.Deg2Rad;
            return new Vector3(size * Mathf.Cos(angle), height, size * Mathf.Sin(angle));
        }
        readonly List<Vector3> vertices = new();
        readonly List<int> triangles = new();
        readonly List<Vector2> uvs = new();
        void CombineFaces()
        {
            vertices.Clear(); triangles.Clear(); uvs.Clear();

            for (int i = 0; i < faces.Count; i++)
            {
                vertices.AddRange(faces[i].vertices);
                uvs.AddRange(faces[i].uvs);

                int offset = (4 * i);
                foreach (var tri in faces[i].triangles) triangles.Add(tri + offset);
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.RecalculateNormals();
        }
    }
    public struct Face
    {
        public readonly List<Vector3> vertices;
        public readonly List<int> triangles;
        public readonly List<Vector2> uvs;
        public Face(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
        {
            this.vertices = vertices;
            this.triangles = triangles;
            this.uvs = uvs;
        }
    }

}