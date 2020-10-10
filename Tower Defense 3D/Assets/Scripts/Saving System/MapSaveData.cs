using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapSaveData
{
    [Serializable]
    public class SaveableObject
    {
        [SerializeField] public int id;

        [SerializeField] public float positionX;
        [SerializeField] public float positionY;
        [SerializeField] public float positionZ;

        [SerializeField] public float rotationX;
        [SerializeField] public float rotationY;
        [SerializeField] public float rotationZ;

        [SerializeField] public float scaleX;
        [SerializeField] public float scaleY;
        [SerializeField] public float scaleZ;

        [SerializeField] public string resourcePath;
    }

    [SerializeField] public List<SaveableObject> saveableObjects = new List<SaveableObject>();
}