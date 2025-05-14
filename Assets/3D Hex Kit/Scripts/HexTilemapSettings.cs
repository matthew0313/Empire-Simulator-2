using System.Collections.Generic;
using UnityEngine;

namespace HexKit3D
{
    [CreateAssetMenu(fileName = "Hex Tilemap Settings", menuName = "3D Hex Kit/Tilemap Settings")]
    public class HexTilemapSettings : ScriptableObject
    {
        [Header("Settings")]
        [SerializeField] float m_tileSize = 1.0f;
        [SerializeField] float m_maxHeightDifference = 1.1f;
        public float tileSize => m_tileSize;
        public float maxHeightDifference => m_maxHeightDifference;
    }
}
/*#if UNITY_EDITOR
namespace HexKit3D.Editor
{
    using System;
    using UnityEditor;
    [CustomPropertyDrawer(typeof(HexTilemapSettings))]
    public class HexTilemapSettings_Drawer : PropertyDrawer
    {
        const float buttonWidth = 120;
        const float padding = 2;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            if (property.objectReferenceValue == null)
            {
                Rect valueRect = position;
                valueRect.width -= buttonWidth + padding;
                EditorGUI.PropertyField(valueRect, property, GUIContent.none);


                Rect buttonRect = position;
                buttonRect.x += position.width - buttonWidth;
                buttonRect.width = buttonWidth;
                if (GUI.Button(buttonRect, "Create"))
                {
                    HexTilemapSettings asset = ScriptableObject.CreateInstance<HexTilemapSettings>();

                    AssetDatabase.CreateAsset(asset, "Assets/New Tilemap Settings.asset");
                    AssetDatabase.SaveAssets();
                    EditorUtility.FocusProjectWindow();

                    property.objectReferenceValue = asset;
                }
            }
            else
            {
                EditorGUI.PropertyField(position, property, GUIContent.none);
            }
            EditorGUI.EndProperty();
        }
    }
}
#endif*/