using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MapSaveManager : MonoBehaviour
{
    [SerializeField] GameObject pathwayPrefab;

    private MapSaveData _mapSaveData;

    private void Awake()
    {
        _mapSaveData = new MapSaveData();

        //initialize checkpoint Pathway for every map
        var pathwayObject = Instantiate(pathwayPrefab, transform);
        var pathwayScript = pathwayObject.GetComponent<Pathway>();
        pathwayScript.Initialize(this, 0);
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

    public void LoadMapData(bool isLoadingInEditor, List<uint> playerIds, string mapName = "defaultGameMap", bool isCustomMap = true)
    {
        ClearScene();

        if (isCustomMap)
        {
            FileManager.LoadFile(FileManager.MapPath, mapName, out _mapSaveData);
        }
        else
        {
            FileManager.LoadResourceFile(FileManager.ResourceMapPath, mapName, out _mapSaveData);
        }

        if (_mapSaveData == null)
            return;

        List<GameObject> gameObjects = new List<GameObject>();
        List<int> objectsToRemove = new List<int>();

        //load map instance for every player
        //in case of editor (no network players! no network manager!) load once
        int numberOfMapInstances = 1;

        List<NetworkPlayer> networkPlayers = new List<NetworkPlayer>();
        if (isLoadingInEditor == false)
        {
            numberOfMapInstances = playerIds.Count;

            var temp = FindObjectsOfType<NetworkPlayer>();

            for (int i = 0; i < playerIds.Count; i++)
            {
                for (int j = 0; j < temp.Length; j++)
                {
                    if (playerIds[i] == temp[j].GetComponent<NetworkIdentity>().netId)
                    {
                        networkPlayers.Add(temp[j]);
                    }
                }
            }
        }

        for (int i = 0; i < numberOfMapInstances; i++)
        {
            uint playerId = 0;
            if (playerIds != null)
            {
                playerId = playerIds[i];
            }

            LoadMapInstance(playerId, gameObjects, objectsToRemove);
        }

        for (int i = objectsToRemove.Count - 1; i >= 0; i--)
            _mapSaveData.RemoveObjectAt(objectsToRemove[i]);

        _mapSaveData.InitializeObjects(isLoadingInEditor, gameObjects, networkPlayers);

        StartCoroutine(NotifyAboutMapLoaded(isLoadingInEditor));
    }

    private void LoadMapInstance(uint playerId, List<GameObject> gameObjects, List<int> objectsToRemove)
    {
        //initialize checkpoint Pathway for every map
        var pathwayObject = Instantiate(pathwayPrefab, transform);
        var pathwayScript = pathwayObject.GetComponent<Pathway>();
        pathwayScript.Initialize(this, playerId);

        int index = 0;
        //instantiates all objects from map data
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
                checkpoint.SetPathway(pathwayScript);
            }

            //if (gameObject.TryGetComponent(out PlacementRuleHandler placementRuleHandler))
            //{
            //    placementRuleHandler.OnObjectPlaced();
            //}

            gameObjects.Add(gameObject);

            index++;
        }
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
