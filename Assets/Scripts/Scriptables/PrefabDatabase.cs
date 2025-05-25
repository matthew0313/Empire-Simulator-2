using UnityEngine;
using System.Collections.Generic;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Item Data", menuName = "Scriptables/Prefab DB")]
public class PrefabDatabase : ScriptableObject
{
    [SerializeField] List<PrefabDatabaseElement> elements = new();
    public IEnumerable<GameObject> GetPrefabs()
    {
        foreach(var i in elements)
        {
            if (i.type == PrefabDatabaseElementMode.Prefab) yield return i.prefab;
            else if(i.type == PrefabDatabaseElementMode.PrefabDatabase)
            {
                foreach (var k in i.prefabDatabase.GetPrefabs()) yield return k;
            }
        }
    }
    public GameObject IndexToPrefab(int index)
    {
        foreach(var i in GetPrefabs())
        {
            if (index-- == 0) return i;
        }
        throw new Exception("Index out of database range");
    }
    public int PrefabToIndex(GameObject prefab)
    {
        int index = 0;
        foreach(var i in GetPrefabs())
        {
            if (i == prefab) return index;
            index++;
        }
        throw new Exception("Unregistered Prefab");
    }
}
[System.Serializable]
public struct PrefabDatabaseElement
{
    public PrefabDatabaseElementMode type;
    public GameObject prefab;
    public PrefabDatabase prefabDatabase;
}
[System.Serializable]
public enum PrefabDatabaseElementMode
{
    Prefab = 0,
    PrefabDatabase = 1
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(PrefabDatabaseElement))]
public class PrefabDBElement_Drawer : PropertyDrawer
{
    const int typeWidth = 110;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(new Rect(position) { width = typeWidth }, property.FindPropertyRelative("type"), GUIContent.none);

        if (property.FindPropertyRelative("type").enumValueIndex == 0)
        {
            EditorGUI.PropertyField(new Rect(position) { x = position.x + typeWidth + 3, width = position.width - typeWidth - 3 }, property.FindPropertyRelative("prefab"), GUIContent.none);
        }
        else if (property.FindPropertyRelative("type").enumValueIndex == 1)
        {
            EditorGUI.PropertyField(new Rect(position) { x = position.x + typeWidth + 3, width = position.width - typeWidth - 3 }, property.FindPropertyRelative("prefabDatabase"), GUIContent.none);
        }
    }
}
#endif
