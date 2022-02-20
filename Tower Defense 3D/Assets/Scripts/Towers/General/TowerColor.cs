using System;
using System.Collections.Generic;
using UnityEngine;

public class TowerColor : MonoBehaviour
{
    [Serializable]
    public class MeshRendererMaterial
    {
        [SerializeField] public MeshRenderer meshRenderer;
        [SerializeField] public int playerColorIndex;
    }

    [SerializeField] List<MeshRendererMaterial> meshRendererMaterials;

    public void ChangeTowerColor(Color color)
    {
        foreach (var meshMaterial in meshRendererMaterials)
        {
            meshMaterial.meshRenderer.materials[meshMaterial.playerColorIndex].SetColor("_BaseColor", color);
        }
    }
}