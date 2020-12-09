using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BrushObjectsHolder : MonoBehaviour
{
    private List<GameObject> _objectsToPlace;
    private int _objectLayer;

    private int brushSize;

    private PlacementValidator _placementValidator;

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

        var placementValidatorSize = objectBounds.size * brushSize;

        //resize placement validator collider

        //shift objects by half of placementValidatorSize to left and forward
        var startingX = (brushSize - 1) * objectBounds.size.x / 2;
        var startingZ = (brushSize - 1) * objectBounds.size.z / 2;

        var shiftX = objectBounds.size.x;
        var shiftZ = objectBounds.size.z;

        int index = 0;
        for (int i = 0; i < brushSize; i++)
        {
            for (int j = 0; j < brushSize; j++)
            {
                var position = _objectsToPlace[index].transform.position;

                position.x = startingX + shiftX * i;
                position.z = startingZ + shiftZ * j;

                _objectsToPlace[index].transform.position = position;

                index++;
            }
        }
    }
}