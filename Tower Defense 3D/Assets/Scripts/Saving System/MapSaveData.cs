﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class MapSaveData
{
    [SerializeField] List<SaveableObject> saveableObjects = new List<SaveableObject>();

    [SerializeField] public GridSettings GridSettings { get; set; }

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

    public void InitializeObjects(List<GameObject> gameObjects, List<NetworkPlayer> networkPlayers)
    {
        int index = 0;

        /*
         * inner foreach iterates over all objects of currently loaded map
         * and initializes all gameObjects - position, rotation, etc
         * 
         * do this for every player - because gameObjects list contains "numberOfPlayers" amount of map objects
         */

        for (int i = 0; i < networkPlayers.Count; i++)
        {
            int xShift = (GridSettings.sizeX + (int)GridSettings.cellSize) * i;
            int zShift = (GridSettings.sizeZ + (int)GridSettings.cellSize) * i;

            networkPlayers[i].Boundaries = new NetworkPlayer.PlayersBoundaries
            {
                MinX = 0 + xShift,
                MaxX = GridSettings.sizeX + xShift,
                MinZ = 0,
                MaxZ = GridSettings.sizeZ
            };

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

                position.x = obj.positionX + xShift;
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