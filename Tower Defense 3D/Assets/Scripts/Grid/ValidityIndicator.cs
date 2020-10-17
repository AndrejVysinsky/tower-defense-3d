using System.Collections.Generic;
using UnityEngine;

public class ValidityIndicator
{
    private readonly Material _validPlacementMaterial;
    private readonly Material _invalidPlacementMaterial;

    private List<Material> _originalMaterials;

    private MeshRenderer _objectMeshRenderer;

    public ValidityIndicator(Material validPlacementMaterial, Material invalidPlacementMaterial)
    {
        _validPlacementMaterial = validPlacementMaterial;
        _invalidPlacementMaterial = invalidPlacementMaterial;

        _originalMaterials = new List<Material>();
    }

    public void RegisterObjectMaterials(GameObject gameObject)
    {
        _objectMeshRenderer = gameObject.GetComponent<MeshRenderer>();

        _originalMaterials.Clear();       
        _originalMaterials.AddRange(_objectMeshRenderer.materials);
    }

    public void SwitchBackObjectMaterials()
    {
        if (_objectMeshRenderer == null)
            return;

        _objectMeshRenderer.materials = _originalMaterials.ToArray();
    }

    public void SetMaterial(bool isValidPlacement)
    {
        if (_objectMeshRenderer == null)
            return;

        if (isValidPlacement)
        {
            SetMaterial(_validPlacementMaterial);
        }
        else
        {
            SetMaterial(_invalidPlacementMaterial);
        }
    }

    private void SetMaterial(Material material)
    {
        var materials = _objectMeshRenderer.materials;

        for (int i = 0; i < materials.Length; i++)
        {
            materials[i] = material;
        }

        _objectMeshRenderer.materials = materials;
    }
}
