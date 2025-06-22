using System.Collections.Generic;
using UnityEngine;

namespace HexKit3D
{
    [RequireComponent(typeof(MeshCollider), typeof(HexRenderer))]
    public class HexCollider : MonoBehaviour
    {
        MeshCollider m_meshCollider;
        public MeshCollider meshCollider
        {
            get
            {
                if(m_meshCollider == null) m_meshCollider = GetComponent<MeshCollider>();
                return m_meshCollider;
            }
        }
        HexRenderer m_target;
        public HexRenderer target
        {
            get
            {
                if(m_target == null) m_target = GetComponent<HexRenderer>();
                return m_target;
            }
        }
        private void OnEnable()
        {
            meshCollider.sharedMesh = target.meshFilter.mesh;
        }
    }
}