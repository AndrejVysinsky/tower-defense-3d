using System.Collections.Generic;
using UnityEngine;

public class ValidityIndicator
{
    private readonly Material _validPlacementMaterial;
    private readonly Material _invalidPlacementMaterial;

    private List<MaterialSwitcher> _materialSwitchers;

    public ValidityIndicator(Material validPlacementMaterial, Material invalidPlacementMaterial)
    {
        _validPlacementMaterial = validPlacementMaterial;
        _invalidPlacementMaterial = invalidPlacementMaterial;

        _materialSwitchers = new List<MaterialSwitcher>();
    }

    public void RegisterObjectMaterials(GameObject gameObject)
    {
        _materialSwitchers.Clear();

        if (gameObject.TryGetComponent(out MeshRendererContainer meshRendererContainer))
        {
            for (int i = 0; i < meshRendererContainer.MeshRenderers.Count; i++)
            {
                _materialSwitchers.Add(new MaterialSwitcher(meshRendererContainer.MeshRenderers[i]));
            }
        }
        else if (gameObject.TryGetComponent(out MeshRenderer meshRenderer))
        {
            _materialSwitchers.Add(new MaterialSwitcher(meshRenderer));
        }
        else
        {
            Debug.Log("Cant switch materials because object does not have MeshRenderer");
        }
    }

    public void SwitchBackObjectMaterials()
    {
        _materialSwitchers.ForEach(x => x.SetOriginalMaterials());
    }

    public void SetMaterial(bool isValidPlacement)
    {
        if (isValidPlacement)
        {
            _materialSwitchers.ForEach(x => x.SetMaterial(_validPlacementMaterial));
        }
        else
        {
            _materialSwitchers.ForEach(x => x.SetMaterial(_invalidPlacementMaterial));
        }
    }
}
