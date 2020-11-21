using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlacementRule
{
    [SerializeField] float rotation;
    [SerializeField] List<DirectionRule> rules;

    public float Rotation => rotation;
    public List<DirectionRule> Rules => rules;
}