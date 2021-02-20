using System;
using UnityEngine;

[Serializable]
public class SaveableObject
{
    [SerializeField] public int id;
    [SerializeField] public int layer;

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

    public SaveableObject()
    {
    }

    public SaveableObject(SaveableObject saveableObject)
    {
        id = saveableObject.id;
        layer = saveableObject.layer;

        positionX = saveableObject.positionX;
        positionY = saveableObject.positionY;
        positionZ = saveableObject.positionZ;

        rotationX = saveableObject.rotationX;
        rotationY = saveableObject.rotationY;
        rotationZ = saveableObject.rotationZ;

        scaleX = saveableObject.scaleX;
        scaleY = saveableObject.scaleY;
        scaleZ = saveableObject.scaleZ;

        resourcePath = saveableObject.resourcePath;
    }
}
