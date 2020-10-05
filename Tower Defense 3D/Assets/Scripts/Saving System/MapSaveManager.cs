using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using static MapSaveData;

public class MapSaveManager : MonoBehaviour
{
    private MapSaveData _mapSaveData;

    private void Awake()
    {
        _mapSaveData = new MapSaveData();
    }

    public void LoadData()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            _mapSaveData = (MapSaveData)bf.Deserialize(file);
            file.Close();

            CreateObjects();
        }
        else
        {
            Debug.Log("No saved data available!");
        }
    }

    public void SaveData()
    {
        RegisterAllChildren();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, _mapSaveData);
        file.Close();
    }

    public void ClearScene()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        _mapSaveData.terrainObjects.Clear();
    }

    private void CreateObjects()
    {
        _mapSaveData.terrainObjects.ForEach(x =>
        {
            var gameObject = new GameObject();
            
            gameObject.transform.parent = transform;
            gameObject.transform.position = x.position;
            
            var meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter = x.meshFilter;

            var meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer = x.meshRenderer;

            var collider = gameObject.AddComponent<Collider>();
            collider = x.collider;
        });
    }

    private void RegisterAllChildren()
    {
        _mapSaveData.terrainObjects.Clear();

        foreach (Transform child in transform)
        {
            _mapSaveData.terrainObjects.Add(new TerrainObject()
            {
                position = child.position,
                meshFilter = child.gameObject.GetComponent<MeshFilter>(),
                meshRenderer = child.gameObject.GetComponent<MeshRenderer>(),
                collider = child.gameObject.GetComponent<Collider>()
            });
        }
    }
}
