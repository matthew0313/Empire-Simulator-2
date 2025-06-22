using System.Collections.Generic;
using UnityEngine;

public class MaterialSetter : MonoBehaviour
{
    List<(MeshRenderer, Material)> m_meshes;
    List<(MeshRenderer, Material)> meshes
    {
        get
        {
            if(m_meshes == null)
            {
                m_meshes = new();
                foreach(var i in gameObject.GetComponentsInChildren<MeshRenderer>())
                {
                    meshes.Add((i, i.material));
                }
            }
            return m_meshes;
        }
    }
    public void SetMaterial(Material material)
    {
        foreach(var i in meshes)
        {
            i.Item1.material = material;
        }
    }
    public void Revert()
    {
        foreach(var i in meshes)
        {
            i.Item1.material = i.Item2;
        }
    }
}