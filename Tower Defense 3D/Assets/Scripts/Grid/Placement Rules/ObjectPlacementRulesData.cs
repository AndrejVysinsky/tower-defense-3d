using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Object Placement Rules", menuName = "Rules/Object Placement Rules")]
public class ObjectPlacementRulesData : ScriptableObject
{
    [Tooltip("True -> Used for path blocks to connect and form a road, but ignore terrain around them.")]
    [SerializeField] bool areNotConnectedSidesTerrain;
    [SerializeField] ObjectPlacementRules objectRules;

    public ObjectPlacementRules ObjectRules => objectRules;
}