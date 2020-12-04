using System.Collections.Generic;
using UnityEngine;

public class MaterialSwitcher
{
    private List<Material> _originalMaterials;
    private MeshRenderer _objectMeshRenderer;

    public MaterialSwitcher(MeshRenderer meshRenderer)
    {
        _objectMeshRenderer = meshRenderer;

        _originalMaterials = new List<Material>();
        _originalMaterials.AddRange(meshRenderer.materials);
    }

    public void SetOriginalMaterials()
    {
        if (_objectMeshRenderer == null)
            return;

        _objectMeshRenderer.materials = _originalMaterials.ToArray();
    }

    public void SetMaterial(Material material)
    {
        var materials = _objectMeshRenderer.materials;

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = material;
        }

        _objectMeshRenderer.materials = materials;
    }
}