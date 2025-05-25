using System;
using System.Collections.Generic;
using HexKit3D;
using Unity.VisualScripting;
using UnityEngine;

public class Island : MonoBehaviour, ISavable
{
    [Header("Island")]
    [SerializeField] string m_islandName;
    public string islandName => m_islandName;

    [Header("Hex Map")]
    [SerializeField] HexTilemap m_tilemap;
    [SerializeField] HexTile m_spawnPoint;
    public HexTilemap tilemap => m_tilemap;

    [Header("Movement Bounds")]
    [SerializeField] Vector3 moveBoundsCenter, m_moveBoundsMin, m_moveBoundsMax;
    public Vector3 moveBoundsMin => transform.position + moveBoundsCenter + m_moveBoundsMin;
    public Vector3 moveBoundsMax => transform.position + moveBoundsCenter + m_moveBoundsMax;

    [Header("Map Elements")]
    [SerializeField] List<FixedMapElement> fixedMapElements = new();
    [SerializeField] List<PlacedMapElement> placedMapElements = new();
    public Action onMapElementChange;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube((moveBoundsMax + moveBoundsMin) / 2.0f, moveBoundsMax - moveBoundsMin);
    }
    private void Awake()
    {

    }
    public IEnumerable<MapElement> MapElements()
    {
        foreach (var i in fixedMapElements) yield return i;
        foreach (var i in placedMapElements) yield return i;
    }
    public IEnumerable<MapElement> SearchElements(Func<MapElement, bool> predicate)
    {
        foreach(var i in MapElements())
        {
            if (predicate.Invoke(i)) yield return i;
        }
    }
    readonly List<MapElement> searchElementsList = new();
    public IEnumerable<MapElement> SearchElements(Func<MapElement, bool> predicate, Func<MapElement, MapElement, int> comparison)
    {
        searchElementsList.Clear();
        foreach (var i in SearchElements(predicate)) searchElementsList.Add(i);
        searchElementsList.Sort((a, b) => comparison.Invoke(a, b));
        foreach(var i in searchElementsList) yield return i;
    }
    public void PlaceElement(PlacedMapElement element)
    {
        placedMapElements.Add(element);
        onMapElementChange?.Invoke();
    }
    public void Save(SaveData data)
    {
        IslandSave save = new();
        foreach(var i in fixedMapElements)
        {
            save.fixedMapElements.Add(i.id.value, i.Save());
        }
        foreach (var i in placedMapElements)
        {
            save.placedMapElements.Add(new()
            {
                id = i.id.value,
                prefabID = GameManager.Instance.placedMapElementDB.PrefabToIndex(i.prefabOrigin),
                position = i.placedTile.position,
                rotation = i.transform.rotation,
                data = i.Save()
            });
        }
        data.islandSaves[islandName] = save;
    }

    public void Load(SaveData data)
    {
        IslandSave save = data.islandSaves[islandName];
        foreach(var i in fixedMapElements)
        {
            if (save.fixedMapElements.TryGetValue(i.id.value, out DataUnit tmp))
            {
                i.Load(tmp);
            }
        }
        foreach(var i in save.placedMapElements)
        {
            if(tilemap.TryGetTile(i.position, out MapTile tile))
            {
                PlacedMapElement tmp = Instantiate(GameManager.Instance.placedMapElementDB.IndexToPrefab(i.prefabID), tile.transform.position, i.rotation).GetComponent<PlacedMapElement>();
                tmp.id.value = i.id;
                placedMapElements.Add(tmp);
                tmp.Load(i.data);
            }
        }
    }
}
[System.Serializable]
public class IslandSave
{
    public SerializableDictionary<string, DataUnit> fixedMapElements = new();
    public List<PlacedMapElementSave> placedMapElements = new();
}