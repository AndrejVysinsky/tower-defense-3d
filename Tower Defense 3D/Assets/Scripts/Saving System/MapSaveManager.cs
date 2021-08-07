using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapSaveManager : MonoBehaviour
{
    [SerializeField] GameObject pathwayPrefab;

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

    public string[] GetAllMaps()
    {
        return FileManager.GetFiles(FileManager.MapPath);
    }

    public void LoadMapData(bool isLoadingInEditor, List<NetworkPlayer> networkPlayers, string mapName = "defaultGameMap")
    {
        ClearScene();

        FileManager.LoadFile(FileManager.MapPath, mapName, out _mapSaveData);

        if (_mapSaveData == null)
            return;

        List<GameObject> gameObjects = new List<GameObject>();
        List<int> objectsToRemove = new List<int>();

        //inner foreach instantiates all objects from map data
        //do this for every player - so everyone has own map
        for (int i = 0; i < networkPlayers.Count; i++)
        {
            //initialize checkpoint Pathway for every map
            var pathwayObject = Instantiate(pathwayPrefab, transform);
            var pathwayScript = pathwayObject.GetComponent<Pathway>();
            pathwayScript.Initialize(this, i);

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

                if (gameObject.TryGetComponent(out Checkpoint checkpoint))
                {
                    checkpoint.Pathway = pathwayScript;
                }

                //if (gameObject.TryGetComponent(out PlacementRuleHandler placementRuleHandler))
                //{
                //    placementRuleHandler.OnObjectPlaced();
                //}

                gameObjects.Add(gameObject);

                index++;
            }
        }

        for (int i = objectsToRemove.Count - 1; i >= 0; i--)
            _mapSaveData.RemoveObjectAt(objectsToRemove[i]);

        _mapSaveData.InitializeObjects(gameObjects, networkPlayers);

        StartCoroutine(NotifyAboutMapLoaded(isLoadingInEditor));
    }

    IEnumerator NotifyAboutMapLoaded(bool isLoadingInEditor)
    {
        yield return new WaitForEndOfFrame();

        EventManager.ExecuteEvent<IMapLoaded>((x, y) => x.OnMapBeingLoaded(_mapSaveData, isLoadingInEditor));
    }

    public void SaveMapData()
    {
        EventManager.ExecuteEvent<IMapSaved>((x, y) => x.OnMapBeingSaved(_mapSaveData));

        FileManager.SaveFile(FileManager.MapPath, "gamesave.save", _mapSaveData);
    }

    public void SaveMapData(string mapName)
    {
        EventManager.ExecuteEvent<IMapSaved>((x, y) => x.OnMapBeingSaved(_mapSaveData));

        FileManager.SaveFile(FileManager.MapPath, mapName, _mapSaveData);
    }

    public void ClearScene()
    {
        EventManager.ExecuteEvent<IMapCleared>((x, y) => x.OnMapBeingCleared());

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        if (_mapSaveData == null)
            return;

        _mapSaveData.RemoveAllObjects();
    }

    public void ObjectPlaced(GameObject gameObject, GameObject prefab)
    {
        if (_mapSaveData == null)
            _mapSaveData = new MapSaveData();

        _mapSaveData.ObjectPlaced(gameObject, prefab);
    }

    public void ObjectLayerUpdated(GameObject gameObject)
    {
        if (_mapSaveData == null)
            _mapSaveData = new MapSaveData();

        _mapSaveData.ObjectLayerUpdated(gameObject);
    }

    public void ObjectRemoved(int gameObjectID)
    {
        if (_mapSaveData == null)
            _mapSaveData = new MapSaveData();

        _mapSaveData.ObjectRemoved(gameObjectID);
    }
}
