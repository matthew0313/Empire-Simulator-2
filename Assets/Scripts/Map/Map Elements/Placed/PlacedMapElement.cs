using System.Collections.Generic;
using HexKit3D;
using UnityEngine;

public abstract class PlacedMapElement : MapElement
{
    [Header("Placed map element")]
    [SerializeField] string m_id;
    public string id => m_id;

    readonly List<(MeshRenderer, List<Material>)> meshes = new();
    protected virtual void Awake()
    {
        SearchMeshes();
    }
    void SearchMeshes()
    {
        Debug.Log(gameObject.name);
        foreach (var i in GetComponentsInChildren<MeshRenderer>())
        {
            if (i.gameObject.CompareTag("Indicator")) continue;
            meshes.Add((i, new List<Material>(i.materials)));
        }
    }
    public void SetMaterial(Material material)
    {
        foreach (var i in meshes)
        {
            List<Material> tmp = new();
            for (int k = 0; k < i.Item1.materials.Length; k++)
            {
                tmp.Add(material);
            }
            i.Item1.SetMaterials(tmp);
        }
    }
    public void RevertMaterial()
    {
        foreach (var i in meshes)
        {
            i.Item1.SetMaterials(i.Item2);
        }
    }
}
[System.Serializable]
public class PlacedMapElementSave
{
    public string id;
    public Vector3 position;
    public Quaternion rotation;
    public DataUnit data;
}