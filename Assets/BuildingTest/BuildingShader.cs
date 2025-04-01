using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshRenderer))]
public class BuildingShader : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    [SerializeField]
    private Vector2 size = Vector2.one;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.SetVector("_BoundsMin", meshRenderer.bounds.min);
        meshRenderer.material.SetVector("_BoundsMax", meshRenderer.bounds.max);
        meshRenderer.material.SetVector("_Size", size);
    }
}
