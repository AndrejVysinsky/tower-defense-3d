using System.Collections.Generic;
using UnityEngine;

public class MaterialSwitcher
{
    private List<Material> _originalMaterials;
    private MeshRenderer _objectMeshRenderer;

    private Material[] _newMaterials;

    public MaterialSwitcher(MeshRenderer meshRenderer)
    {
        _objectMeshRenderer = meshRenderer;

        _originalMaterials = new List<Material>();
        _originalMaterials.AddRange(meshRenderer.materials);

        _newMaterials = new Material[_originalMaterials.Count];
    }

    public void SetOriginalMaterials()
    {
        if (_objectMeshRenderer == null)
            return;

        _objectMeshRenderer.materials = _originalMaterials.ToArray();
    }

    public void SetMaterial(Material material)
    {
        for (int i = 0; i < _newMaterials.Length; i++)
        {
            _newMaterials[i] = material;
        }
        _objectMeshRenderer.materials = _newMaterials;
    }
}