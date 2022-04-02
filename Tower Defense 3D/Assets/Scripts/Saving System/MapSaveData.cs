using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class MapSaveData
{
    [SerializeField] List<SaveableObject> saveableObjects = new List<SaveableObject>();

    [SerializeField] public GridSettings GridSettings { get; set; }

    [SerializeField] string mapHash;

    public string GetMapHash()
    {
        return mapHash;
    }

    public void GenerateMapHash(string mapName, DateTime saveTime)
    {
        var hash = new Hash128();
        hash.Append(mapName);
        hash.Append(saveTime.ToString());
        mapHash = hash.ToString();
    }

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

    public void InitializeObjects(bool isLoadingInEditor, int numberOfMapInstances, List<GameObject> gameObjects, List<NetworkPlayer> networkPlayers)
    {
        var mapBoundaries = new Boundaries();

        int boundariesShift = GridSettings.sizeX + (int)GridSettings.cellSize;
        if (isLoadingInEditor == false)
        {
            if (numberOfMapInstances == 1)
            {
                boundariesShift = 0;
            }

            for (int i = 0; i < networkPlayers.Count; i++)
            {
                if (networkPlayers[i].PlayerBoundaries == null)
                    networkPlayers[i].PlayerBoundaries = new Boundaries();

                networkPlayers[i].PlayerBoundaries.SetBoundaries(0 + boundariesShift * i, GridSettings.sizeX + boundariesShift * i, 0, GridSettings.sizeZ);

                mapBoundaries.ExtendBoundariesTo(networkPlayers[i].PlayerBoundaries);
            }

            for (int i = 0; i < networkPlayers.Count; i++)
            {
                networkPlayers[i].SetMapBoundaries(mapBoundaries);
            }
        }

        /*
         * inner foreach iterates over all objects of currently loaded map
         * and initializes all gameObjects - position, rotation, etc
         * 
         * do this for every player - because gameObjects list contains "numberOfPlayers" amount of map objects
         */

        int index = 0;

        var mapShift = GridSettings.sizeX + (int)GridSettings.cellSize;
        for (int i = 0; i < numberOfMapInstances; i++) 
        {
            saveableObjects.ForEach(obj =>
            {
                if (obj is SaveableCheckpoint saveableCheckpoint && gameObjects[index].TryGetComponent(out Checkpoint checkpoint))
                {
                    checkpoint.SaveableCheckpoint = saveableCheckpoint;
                }

                //kind of useless because if there is more than 1 player it gets overriden in next iteration
                obj.id = gameObjects[index].GetInstanceID();

                gameObjects[index].layer = obj.layer;

                var position = gameObjects[index].transform.position;

                position.x = obj.positionX + mapShift * i;
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
    }

    public SaveableObject GetSaveableObject(int id)
    {
        return saveableObjects.FirstOrDefault(obj => obj.id == id);
    }

    public void UpdateSaveableObject(int id, SaveableObject newSaveableObject)
    {
        for (int i = 0; i < saveableObjects.Count; i++)
        {
            if (saveableObjects[i].id == id)
            {
                saveableObjects[i] = newSaveableObject;
                break;
            }
        }
    }

    public void RemoveAllObjects()
    {
        saveableObjects.Clear();
    }
}