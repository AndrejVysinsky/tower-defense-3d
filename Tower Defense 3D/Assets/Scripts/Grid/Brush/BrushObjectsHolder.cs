using System.Collections.Generic;
using UnityEngine;

public class BrushObjectsHolder : MonoBehaviour
{
    private List<GameObject> _objectsToPlace;
    private GameObject _originalPrefab;

    private PlacementValidator _placementValidator;

    public bool IsHoldingObjects => _objectsToPlace.Count > 0;

    private void Awake()
    {
        _objectsToPlace = new List<GameObject>();
        _placementValidator = GetComponent<PlacementValidator>();
    }

    public void InstantiateObjects(GameObject toPlacePrefab, int brushSize)
    {
        _originalPrefab = toPlacePrefab;

        for (int i = 0; i < brushSize * brushSize; i++)
        {
            var toPlace = Instantiate(toPlacePrefab, transform);

            toPlace.layer = (int)LayerEnum.IgnoreRayCast;

            _objectsToPlace.Add(toPlace);
        }
        
        var objectBounds = _objectsToPlace[0].GetComponent<Collider>().bounds;

        var placementValidatorSize = objectBounds.size;
        placementValidatorSize.x *= brushSize;
        placementValidatorSize.z *= brushSize;

        //resize placement validator collider
        _placementValidator.RegisterChildren(_objectsToPlace, placementValidatorSize);

        //rotated collider size
        var size = _placementValidator.GetRotatedColliderSize(objectBounds.size);

        //shift objects by half of placementValidatorSize to left and forward
        var startingX = - (brushSize - 1) * size.x / 2;
        var startingZ = - (brushSize - 1) * size.z / 2;

        var shiftX = size.x;
        var shiftZ = size.z;

        int index = 0;
        for (int i = 0; i < brushSize; i++)
        {
            for (int j = 0; j < brushSize; j++)
            {
                var position = Vector3.zero;

                position.x = startingX + shiftX * i;
                position.z = startingZ + shiftZ * j;

                _objectsToPlace[index].transform.localPosition = position;

                index++;
            }
        }
    }

    public float GetOriginY()
    {
        var bounds = _placementValidator.GetComponent<Collider>().bounds;

        return transform.position.y + bounds.extents.y - bounds.center.y;
    }

    public bool TryPlaceObjectsOnMap(MapSaveManager map)
    {
        bool success = false;
        for (int i = 0; i < _objectsToPlace.Count; i++)
        {
            /*
                if object implements IUpgradeable, try to start upgrade
                returns false if upgrade did not start (should mean not enough gold) -> skip object placing
             */
            if (_objectsToPlace[i].TryGetComponent(out IUpgradeable upgradeable))
            {
                upgradeable.OnUpgradeStarted(upgradeable.CurrentUpgrade, out bool upgradeStarted);

                if (upgradeStarted == false)
                    continue;
            }

            //check for block visual adaptation (mostly for terrain block connecting)
            //if (_objectsToPlace[i].TryGetComponent(out PlacementRuleHandler placementRuleHandler))
            //{
            //    placementRuleHandler.OnObjectPlaced();
            //}

            if (_objectsToPlace[i].TryGetComponent(out IGridObjectPlaced gridObjectPlaced))
            {
                gridObjectPlaced.OnGridObjectPlaced();
            }

            //switch back layer
            _objectsToPlace[i].layer = _originalPrefab.layer;

            //reparent to map object
            _objectsToPlace[i].transform.parent = map.transform;
            map.ObjectPlaced(_objectsToPlace[i], _originalPrefab);
            success = true;
        }
        return success;
    }    

    public void ClearObjects()
    {
        _placementValidator.DeregisterChildren();
        _objectsToPlace.Clear();
    }

    public void DestroyObjects()
    {
        _placementValidator.DeregisterChildren();

        foreach (var objectToPlace in _objectsToPlace)
        {
            if (objectToPlace.TryGetComponent(out IGridObjectRemoved gridObjectRemoved))
            {
                gridObjectRemoved.OnGridObjectRemoved();
            }

            Destroy(objectToPlace);
        }

        _objectsToPlace.Clear();
    }
}