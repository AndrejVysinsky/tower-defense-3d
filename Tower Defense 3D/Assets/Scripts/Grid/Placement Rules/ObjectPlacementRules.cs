using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectPlacementRules
{
    [SerializeField] GameObject ruleObject;
    [SerializeField] List<PlacementRule> placementRules;

    public GameObject RuleObject => ruleObject;
    public List<PlacementRule> PlacementRules => placementRules;
}