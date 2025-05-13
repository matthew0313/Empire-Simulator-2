using UnityEngine;
using UnityEditor;

namespace HexKit3D.Editor
{
    public static class EditorResources
    {
        static HexRenderer m_hexPreview;
        public static HexRenderer hexPreview
        {
            get
            {
                if (m_hexPreview == null)
                {
                    m_hexPreview = MonoBehaviour.Instantiate(Resources.Load<HexRenderer>("HexPreview"));
                    m_hexPreview.gameObject.hideFlags = HideFlags.HideAndDontSave;
                }
                return m_hexPreview;
            }
        }
        static TextHexRenderer m_hexTextPreviewPrefab;
        public static TextHexRenderer hexTextPreviewPrefab
        {
            get
            {
                if(m_hexTextPreviewPrefab == null)
                {
                    m_hexTextPreviewPrefab = Resources.Load<TextHexRenderer>("HexTextPreview");
                }
                return m_hexTextPreviewPrefab;
            }
        }
    }
}
