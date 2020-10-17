using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapSaveManager : MonoBehaviour
{
    private MapSaveData _mapSaveData;

    private void Awake()
    {
        _mapSaveData = new MapSaveData();
    }

    private void Start()
    {
        if (TryGetComponent(out NavMeshSurface navMeshSurface))
        {
            navMeshSurface.BuildNavMesh();
        }
    }

    public void LoadMapData()
    {
        ClearScene();

        FileManager.LoadFile(FileManager.MapPath, "gamesave.save", out _mapSaveData);

        List<GameObject> gameObjects = new List<GameObject>();

        _mapSaveData.GetResourcePaths().ForEach(path =>
        {
            var gameObject = Instantiate(Resources.Load<GameObject>(path), transform);

            gameObjects.Add(gameObject);
        });

        _mapSaveData.InitializeObjects(gameObjects);
    }

    public void SaveMapData()
    {
        FileManager.SaveFile(FileManager.MapPath, "gamesave.save", _mapSaveData);
    }

    public void ClearScene()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        _mapSaveData.RemoveAllObjects();
    }

    public void ObjectPlaced(GameObject gameObject, GameObject prefab)
    {
        _mapSaveData.ObjectPlaced(gameObject, prefab);
    }

    public void ObjectLayerUpdated(GameObject gameObject)
    {
        _mapSaveData.ObjectLayerUpdated(gameObject);
    }

    public void ObjectRemoved(int gameObjectID)
    {
        _mapSaveData.ObjectRemoved(gameObjectID);
    }
}
