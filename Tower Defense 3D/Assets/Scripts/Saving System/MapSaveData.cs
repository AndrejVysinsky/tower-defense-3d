using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class MapSaveData
{
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
    }

    [SerializeField] List<SaveableObject> saveableObjects = new List<SaveableObject>();

    public List<string> GetResourcePaths()
    {
        return saveableObjects.Select(x => x.resourcePath).ToList();
    }

    public void RemoveObjectAt(int index)
    {
        saveableObjects.RemoveAt(index);
    }

    public void ObjectPlaced(GameObject gameObject, GameObject prefab)
    {
        if (prefab.TryGetComponent(out ResourcePath resourcePath) == false)
        {
            return;
        }

        saveableObjects.Add(new SaveableObject()
        {
            id = gameObject.GetInstanceID(),
            layer = gameObject.layer,

            positionX = gameObject.transform.position.x,
            positionY = gameObject.transform.position.y,
            positionZ = gameObject.transform.position.z,

            rotationX = gameObject.transform.rotation.eulerAngles.x,
            rotationY = gameObject.transform.rotation.eulerAngles.y,
            rotationZ = gameObject.transform.rotation.eulerAngles.z,

            scaleX = gameObject.transform.localScale.x,
            scaleY = gameObject.transform.localScale.y,
            scaleZ = gameObject.transform.localScale.z,

            resourcePath = resourcePath.Path + "/" + prefab.name
        });
    }

    public void ObjectLayerUpdated(GameObject gameObject)
    {
        var saveableObject = saveableObjects.Find(x => x.id == gameObject.GetInstanceID());

        if (saveableObject == null)
            return;

        saveableObject.layer = gameObject.layer;
    }

    public void ObjectRemoved(int gameObjectID)
    {
        saveableObjects.RemoveAll(x => x.id == gameObjectID);
    }

    public void InitializeObjects(List<GameObject> gameObjects)
    {
        int index = 0;

        saveableObjects.ForEach(obj =>
        {
            obj.id = gameObjects[index].GetInstanceID();

            gameObjects[index].layer = obj.layer;

            var position = gameObjects[index].transform.position;

            position.x = obj.positionX;
            position.y = obj.positionY;
            position.z = obj.positionZ;

            gameObjects[index].transform.position = position;

            gameObjects[index].transform.rotation = Quaternion.Euler(obj.rotationX, obj.rotationY, obj.rotationZ);

            //gameObjects[index].transform.Rotate(new Vector3(obj.rotationX, obj.rotationY, obj.rotationZ));

            var scale = gameObjects[index].transform.localScale;

            scale.x = obj.scaleX;
            scale.y = obj.scaleY;
            scale.z = obj.scaleZ;

            gameObjects[index].transform.localScale = scale;

            index++;
        });
    }

    public void RemoveAllObjects()
    {
        saveableObjects.Clear();
    }
}