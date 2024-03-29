﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacementRuleHandler : MonoBehaviour, IMapLoaded
{
    [SerializeField] GameObject defaultObject;
    [SerializeField] List<ObjectPlacementRulesData> objectRulesData;

    private GameObject[] _neighbours = new GameObject[10];
    private int _neighbourCount = 0;

    private Collider[] _colliderBuffer = new Collider[3];

    private float _colliderSize;
    private float _overlapSphereRadius;

    private bool _isPlaced = false;

    private Quaternion _defaultRotation;

    private void OnEnable()
    {
        EventManager.AddListener(gameObject);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(gameObject);
    }

    public void OnObjectPlaced()
    {
        _isPlaced = true;
        _defaultRotation = transform.rotation;

        //_colliderSize = GetComponent<MeshCollider>().bounds.size.x;
        _colliderSize = GetComponent<Collider>().bounds.size.x;
        _overlapSphereRadius = 0.1f;

        StartCoroutine(CheckNeighbours());
    }

    IEnumerator CheckNeighbours()
    {
        yield return new WaitForFixedUpdate();

        var northSouthOffset = new Vector3(0, 0, _colliderSize);
        var eastWestOffset = new Vector3(_colliderSize, 0, 0);
        var aboveUnderOffset = new Vector3(0, _colliderSize, 0);

        CheckDirection(DirectionEnum.NORTH, DirectionEnum.SOUTH, northSouthOffset);
        CheckDirection(DirectionEnum.SOUTH, DirectionEnum.NORTH, -northSouthOffset);

        CheckDirection(DirectionEnum.EAST, DirectionEnum.WEST, eastWestOffset);
        CheckDirection(DirectionEnum.WEST, DirectionEnum.EAST, -eastWestOffset);

        CheckDirection(DirectionEnum.NORTH_EAST, DirectionEnum.SOUTH_WEST, northSouthOffset + eastWestOffset);
        CheckDirection(DirectionEnum.SOUTH_WEST, DirectionEnum.NORTH_EAST, -northSouthOffset - eastWestOffset);

        CheckDirection(DirectionEnum.SOUTH_EAST, DirectionEnum.NORTH_WEST, -northSouthOffset + eastWestOffset);
        CheckDirection(DirectionEnum.NORTH_WEST, DirectionEnum.SOUTH_EAST, northSouthOffset - eastWestOffset);

        CheckDirection(DirectionEnum.UNDER, DirectionEnum.ABOVE, -aboveUnderOffset);
    }

    private void CheckDirection(DirectionEnum checkingDirection, DirectionEnum oppositeDirection, Vector3 offset)
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position + offset, _overlapSphereRadius, _colliderBuffer);

        if (IsNeighbourObjectTerrain(count, out GameObject otherObject))
        {
            AddNeighbour(otherObject, checkingDirection);

            if (otherObject.TryGetComponent(out PlacementRuleHandler ruleHandler))
            {
                ruleHandler.AddNeighbour(gameObject, oppositeDirection);
            }
        }
    }    

    private bool IsNeighbourObjectTerrain(int neighbourCount, out GameObject otherObject)
    {
        for (int i = 0; i < neighbourCount; i++)
        {
            if (_colliderBuffer[i].CompareTag("Terrain"))
            {
                otherObject = _colliderBuffer[i].gameObject;
                return true;
            }
        }
        otherObject = null;
        return false;
    }

    private void OnDestroy()
    {
        if (_isPlaced == false)
            return;

        for (int i = 0; i < _neighbours.Length; i++)
        {
            if (_neighbours[i] != null && _neighbours[i].TryGetComponent(out PlacementRuleHandler placementRuleHandler))
            {
                placementRuleHandler.RemoveNeighbour(gameObject);
            }
        }
    }

    public void AddNeighbour(GameObject neighbourObject, DirectionEnum direction)
    {
        _neighbours[(int)direction] = neighbourObject;
        _neighbourCount++;

        OnNeighbourChanged();
    }

    public void RemoveNeighbour(GameObject neighbourObject)
    {
        for (int i = 0; i < _neighbours.Length; i++)
        {
            if (_neighbours[i] == neighbourObject)
            {
                _neighbours[i] = null;
                _neighbourCount--;
                OnNeighbourChanged();
                break;
            }
        }
    }

    private void OnNeighbourChanged()
    {
        if (defaultObject != null)
            ChangeObjectTo(defaultObject);

        if (objectRulesData.Count == 0)
            return;

        for (int i = 0; i < objectRulesData.Count; i++)
        {
            for (int j = 0; j < objectRulesData[i].ObjectRules.PlacementRules.Count; j++)
            {
                var rules = objectRulesData[i].ObjectRules.PlacementRules[j].Rules;

                //if (HasMoreConnectedSidesThanNeighbours(rules))
                //{
                //    continue;
                //}

                bool ruleValid = true;

                for (int k = 0; k < rules.Count; k++)
                {
                    var rule = rules[k];

                    if (rule.IsConnected)
                    {
                        //rule is connected, so its expecting neighbour in said direction
                        if (_neighbours[(int)rule.Direction] == null)
                        {
                            ruleValid = false;
                            break;
                        }
                    }
                    else
                    {
                        //rule is NOT connected, so direction should be empty (without neighbour)
                        if (_neighbours[(int)rule.Direction] != null)
                        {
                            ruleValid = false;
                            break;
                        }
                    }
                }

                if (ruleValid)
                {
                    ChangeObjectTo(objectRulesData[i].ObjectRules.RuleObject, objectRulesData[i].ObjectRules.PlacementRules[j].Rotation);
                    return;
                }
            }
        }
    }

    private bool HasMoreConnectedSidesThanNeighbours(List<DirectionRule> directionRules)
    {
        int connectedSides = 0;

        for (int i = 0; i < directionRules.Count; i++)
        {
            if (directionRules[i].IsConnected)
                connectedSides++;
        }

        return connectedSides > _neighbourCount;
    }

    private void ChangeObjectTo(GameObject newGameObject, float angle = 0)
    {
        var mesh = newGameObject.GetComponent<MeshFilter>().sharedMesh;

        gameObject.GetComponent<MeshFilter>().mesh = mesh;

        var materials = newGameObject.GetComponent<MeshRenderer>().sharedMaterials;

        gameObject.GetComponent<MeshRenderer>().sharedMaterials = materials;

        //gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;

        var rotation = transform.rotation;

        rotation.eulerAngles = new Vector3(0, angle, 0);

        transform.rotation = rotation;
    }

    public void OnMapBeingLoaded(MapSaveData mapSaveData, bool isLoadingInEditor)
    {
        OnObjectPlaced();
    }
}