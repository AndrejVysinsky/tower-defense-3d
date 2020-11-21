using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Object Placement Rules", menuName = "Rules/Object Placement Rules")]
public class ObjectPlacementRulesData : ScriptableObject
{
    [SerializeField] ObjectPlacementRules objectRules;

    public ObjectPlacementRules ObjectRules => objectRules;
}