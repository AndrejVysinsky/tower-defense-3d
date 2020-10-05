using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapSaveData
{
    [Serializable]
    public class TerrainObject
    {
        [SerializeField] public Vector3 position;
        [SerializeField] public MeshFilter meshFilter;
        [SerializeField] public MeshRenderer meshRenderer;
        [SerializeField] public Collider collider;
    }

    [SerializeField] public List<TerrainObject> terrainObjects = new List<TerrainObject>();
}