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

        List<int> objectsToRemove = new List<int>();

        int index = 0;
        foreach (var path in _mapSaveData.GetResourcePaths())
        {
            var resource = Resources.Load<GameObject>(path);

            if (resource == null)
            {
                Debug.Log($"Resource \"{resource}\" in path \"{path}\" not found");
                objectsToRemove.Add(index);
                continue;
            }

            var gameObject = Instantiate(resource, transform);

            //if (gameObject.TryGetComponent(out PlacementRuleHandler placementRuleHandler))
            //{
            //    placementRuleHandler.OnObjectPlaced();
            //}

            gameObjects.Add(gameObject);

            index++;
        }

        for (int i = objectsToRemove.Count - 1; i >= 0; i--)
            _mapSaveData.RemoveObjectAt(objectsToRemove[i]);

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
