using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnbuildableTerrainMarker : MonoBehaviour
{
    [SerializeField] Material buildableMaterial;
    [SerializeField] Material unbuildableMaterial;

    [SerializeField] MapSaveManager mapSaveManager;

    private Dictionary<GameObject, ValidityIndicator> _terrainObjects;

    private bool _isMarkingActive;

    private void Awake()
    {
        _terrainObjects = new Dictionary<GameObject, ValidityIndicator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _isMarkingActive)
        {
            if (RayCaster.RayCastUIObject(out _))
                return;

            if (RayCaster.RaycastGameObjectWithTag(out RaycastHit hitInfo, "Terrain"))
            {
                SwitchLayerAndMaterial(hitInfo.transform.gameObject);
            }
        }
    }

    private void SwitchLayerAndMaterial(GameObject terrainObject)
    {
        if (_terrainObjects.TryGetValue(terrainObject, out ValidityIndicator validityIndicator))
        {
            if (terrainObject.layer == (int)LayerEnum.Default)
            {
                terrainObject.layer = (int)LayerEnum.UnbuildableTerrain;
            }
            else
            {
                terrainObject.layer = (int)LayerEnum.Default;
            }

            mapSaveManager.ObjectLayerUpdated(terrainObject);

            validityIndicator.SetMaterial(terrainObject.layer != (int)LayerEnum.UnbuildableTerrain);
        }
    }

    public void SwitchMarkingMode()
    {
        _isMarkingActive = !_isMarkingActive;

        if (_isMarkingActive)
        {
            RegisterTerrainObjects();
        }
        else
        {
            SwitchBackMaterials();
        }
    }

    private void RegisterTerrainObjects()
    {
        _terrainObjects.Clear();

        foreach (Transform child in mapSaveManager.transform)
        {
            if (child.CompareTag("Terrain"))
            {
                var validityIndicator = new ValidityIndicator(buildableMaterial, unbuildableMaterial);

                validityIndicator.RegisterObjectMaterials(child.gameObject);

                validityIndicator.SetMaterial(child.gameObject.layer != (int)LayerEnum.UnbuildableTerrain);

                _terrainObjects.Add(child.gameObject, validityIndicator);
            }
        }
    }

    private void SwitchBackMaterials()
    {
        foreach (var value in _terrainObjects.Values)
        {
            value.SwitchBackObjectMaterials();
        }
    }
}