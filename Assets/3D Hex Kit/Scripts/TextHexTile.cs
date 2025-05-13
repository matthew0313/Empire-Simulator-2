using TMPro;
using UnityEngine;

namespace HexKit3D
{
    public class TextHexRenderer : HexRenderer
    {
        [SerializeField] TextMeshPro m_textMesh;
        public TextMeshPro textMesh => m_textMesh;
    }
}