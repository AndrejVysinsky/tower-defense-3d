using System.Collections.Generic;
using UnityEngine;

public class BrushObjectsHolder : MonoBehaviour
{
    private List<GameObject> _objectsToPlace;
    private int _objectLayer;

    private int brushSize = 3;

    private PlacementValidator _placementValidator;

    public bool IsHoldingObjects => _objectsToPlace.Count > 0;
    public int BrushSize => brushSize;

    private void Awake()
    {
        _objectsToPlace = new List<GameObject>();
        _placementValidator = GetComponent<PlacementValidator>();
    }

    public void InstantiateObjects(GameObject toPlacePrefab)
    {
        for (int i = 0; i < brushSize * brushSize; i++)
        {
            var toPlace = Instantiate(toPlacePrefab, transform);

            _objectLayer = toPlace.layer;

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
        var size = Quaternion.Euler(transform.rotation.eulerAngles) * objectBounds.size;

        size.x = Mathf.Abs(size.x);
        size.y = Mathf.Abs(size.y);
        size.z = Mathf.Abs(size.z);

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

    public void TryToConstruct()
    {
        for (int i = 0; i < _objectsToPlace.Count; i++)
        {
            if (_objectsToPlace[i].TryGetComponent(out IUpgradeable upgradeable))
            {
                upgradeable.OnUpgradeStarted(upgradeable.CurrentUpgrade, out bool upgradeStarted);

                if (upgradeStarted == false)
                    return;
            }
        }
    }

    public void TryToPlacementRuleHandler()
    {
        for (int i = 0; i < _objectsToPlace.Count; i++)
        {
            if (_objectsToPlace[i].TryGetComponent(out PlacementRuleHandler placementRuleHandler))
            {
                placementRuleHandler.OnObjectPlaced();
            }
        }
    }

    public void SwitchBackLayer()
    {
        for (int i = 0; i < _objectsToPlace.Count; i++)
        {
            _objectsToPlace[i].layer = _objectLayer;
        }
    }

    public void ReparentToMap(MapSaveManager map, GameObject originalPrefab)
    {
        for (int i = 0; i < _objectsToPlace.Count; i++)
        {
            //var position = _objectsToPlace[i].transform.localPosition;

            //position += transform.position;

            _objectsToPlace[i].transform.parent = map.transform;

            //_objectsToPlace[i].transform.position = position;

            map.ObjectPlaced(_objectsToPlace[i], originalPrefab);
        }
    }

    public void ClearObjects()
    {
        _placementValidator.DeregisterChildren();
        _objectsToPlace.Clear();
    }

    public void DestroyObjects()
    {
        _placementValidator.DeregisterChildren();
        _objectsToPlace.ForEach(x => Destroy(x));
        _objectsToPlace.Clear();
    }
}