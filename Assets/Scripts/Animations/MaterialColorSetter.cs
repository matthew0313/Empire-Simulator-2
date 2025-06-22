using System;
using UnityEngine;

public class MaterialColorSetter : MonoBehaviour
{
    [SerializeField] MeshRenderer target;
    public Color color;
    private void Update()
    {
        target.material.color = color;
    }
}