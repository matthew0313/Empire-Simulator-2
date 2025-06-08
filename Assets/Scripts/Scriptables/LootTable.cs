using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Loot Table", menuName = "Scriptables/Loot Table")]
public class LootTable : ScriptableObject
{
    [SerializeField] LootTableElement[] elements;
    public IEnumerable<ItemIntPair> GetLoot(float luckMultiplier = 1.0f)
    {
        foreach(var i in elements)
        {
            float chance = i.chance * luckMultiplier;
            int count = Mathf.FloorToInt(chance / 100.0f);
            chance -= count * 100.0f;
            if(UnityEngine.Random.Range(0.0f, 100.0f) <= chance)
            {
                count++;
            }
            yield return new() { item = i.item, count = count };
        }
    }
}
[System.Serializable]
public struct LootTableElement
{
    public ItemData item;
    [Tooltip("100 = guranteed 1, 150 = guranteed 1 + 50% 1")] public float chance;
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(LootTableElement))]
public class LootTableElement_Drawer : PropertyDrawer
{
    const int countWidth = 110;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(new Rect(position) { width = position.width - countWidth - 3 }, property.FindPropertyRelative("item"), GUIContent.none);
        EditorGUI.PropertyField(new Rect(position) { x = position.x + position.width - countWidth, width = countWidth }, property.FindPropertyRelative("chance"), GUIContent.none);
    }
}
#endif
