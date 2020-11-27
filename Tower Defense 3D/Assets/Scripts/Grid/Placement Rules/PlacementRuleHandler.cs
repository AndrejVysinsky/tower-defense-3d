using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlacementRuleHandler : MonoBehaviour
{
    [SerializeField] GameObject defaultObject;
    [SerializeField] List<ObjectPlacementRules> objectRules;
    [SerializeField] List<ObjectPlacementRulesData> objectRulesData;

    private GameObject[] _neighbours = new GameObject[10];
    private Collider[] _colliderBuffer = new Collider[3];

    private float _colliderSize;
    private float _overlapSphereRadius;

    private bool _isPlaced = false;

    private Quaternion _defaultRotation;

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

        if (IsNeighbourObjectTerrain(count, out PlacementRuleHandler otherHandler))
        {
            AddNeighbour(otherHandler.gameObject, checkingDirection);
            otherHandler.AddNeighbour(gameObject, oppositeDirection);
        }
    }    

    private bool IsNeighbourObjectTerrain(int neighbourCount, out PlacementRuleHandler placementRuleHandler)
    {
        for (int i = 0; i < neighbourCount; i++)
        {
            if (_colliderBuffer[i].CompareTag("Terrain") && _colliderBuffer[i].TryGetComponent(out PlacementRuleHandler placementHandler))
            {
                placementRuleHandler = placementHandler;
                return true;
            }
        }
        placementRuleHandler = null;
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

        OnNeighbourChanged();
    }

    public void RemoveNeighbour(GameObject neighbourObject)
    {
        for (int i = 0; i < _neighbours.Length; i++)
        {
            if (_neighbours[i] == neighbourObject)
            {
                _neighbours[i] = null;
                OnNeighbourChanged();
                break;
            }
        }
    }

    private void OnNeighbourChanged()
    {
        ChangeObjectTo(defaultObject);

        if (_neighbours[(int)DirectionEnum.ABOVE] != null)
            return;

        //for (int i = 0; i < objectRules.Count; i++)
        //{
        //    for (int j = 0; j < objectRules[i].PlacementRules.Count; j++)
        //    {
        //        var rules = objectRules[i].PlacementRules[j].Rules;

        //        if (rules.Count(x => x.IsConnected) > GetNeighbourCount())
        //        {
        //            continue;
        //        }

        //        bool ruleValid = true;

        //        for (int k = 0; k < rules.Count; k++)
        //        {
        //            var rule = rules[k];

        //            if (rule.IsConnected)
        //            {
        //                //rule is connected, so its expecting neighbour in said direction
        //                if (_neighbours[(int)rule.Direction] == null)
        //                {
        //                    ruleValid = false;
        //                    break;
        //                }
        //            }
        //            else
        //            {
        //                //rule is NOT connected, so direction should be empty (without neighbour)
        //                if (_neighbours[(int)rule.Direction] != null)
        //                {
        //                    ruleValid = false;
        //                    break;
        //                }
        //            }
        //        }

        //        if (ruleValid)
        //        {
        //            ChangeObjectTo(objectRules[i].RuleObject, objectRules[i].PlacementRules[j].Rotation);
        //            break;
        //        }
        //    }
        //}   
        blabla();
    }

    private void blabla()
    {
        for (int i = 0; i < objectRulesData.Count; i++)
        {
            for (int j = 0; j < objectRulesData[i].ObjectRules.PlacementRules.Count; j++)
            {
                var rules = objectRulesData[i].ObjectRules.PlacementRules[j].Rules;

                if (rules.Count(x => x.IsConnected) > GetNeighbourCount())
                {
                    continue;
                }

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

    private void ChangeObjectTo(GameObject newGameObject, float rotation = 0)
    {
        var mesh = newGameObject.GetComponent<MeshFilter>().sharedMesh;

        gameObject.GetComponent<MeshFilter>().mesh = mesh;
        //gameObject.GetComponent<MeshCollider>().sharedMesh = mesh;

        transform.rotation = _defaultRotation;

        if (rotation != 0)
        {
            transform.Rotate(Vector3.up, rotation);
        }
    }

    private int GetNeighbourCount()
    {
        int count = 0;

        for (int i = 0; i < _neighbours.Length; i++)
        {
            if (_neighbours[i] != null)
            {
                count++;
            }
        }
        return count;
    }
}