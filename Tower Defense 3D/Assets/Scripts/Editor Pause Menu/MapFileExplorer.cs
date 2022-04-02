using Mirror;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapFileExplorer : MonoBehaviour
{
    [SerializeField] Transform mapFileExplorerWindow;
    [SerializeField] GameObject mapFilePrefab;
    [SerializeField] MapSaveManager mapManager;

    [SerializeField] GameObject savePanel;
    [SerializeField] GameObject loadPanel;

    [SerializeField] TMP_InputField mapFileNameInput;

    private string[] _mapNames;
    private MapFile _selectedMapFile;

    public void ShowMapFiles()
    {
        _mapNames = mapManager.GetAllMaps();

        while (_mapNames.Length > mapFileExplorerWindow.childCount)
        {
            Instantiate(mapFilePrefab, mapFileExplorerWindow);
        }

        foreach (Transform child in mapFileExplorerWindow)
        {
            child.gameObject.SetActive(false);
        }

        for (int i = 0; i < _mapNames.Length; i++)
        {
            var mapFileObject = mapFileExplorerWindow.GetChild(i).gameObject;

            mapFileObject.SetActive(true);
            mapFileObject.GetComponent<MapFile>().PopulateThumbnailData(_mapNames[i], this);
        }
    }
    
    public void MapFileSelected(MapFile mapFile)
    {
        if (_selectedMapFile != null)
        {
            _selectedMapFile.DeselectFile();
        }

        _selectedMapFile = mapFile;
        mapFileNameInput.text = mapFile.MapName;
    }

    public void SaveMapFile()
    {
        if (mapFileNameInput.text == null)
        {
            Debug.Log("Missing map name.");
            return;
        }

        mapManager.SaveMapData(mapFileNameInput.text);

        ShowMapFiles();
    }

    public void LoadMapFile(bool isLoadingInEditor)
    {
        if (_selectedMapFile == null)
        {
            Debug.Log("Missing map name.");
            return;
        }

        //TODO: this call should not be here - temporary workaround
        var myNetworkManager = (MyNetworkManager)NetworkManager.singleton;

        List<uint> playerIds = null;
        if (myNetworkManager != null)
        {
            playerIds = myNetworkManager.GetPlayerIds();
        }

        mapManager.LoadMapData(isLoadingInEditor, playerIds, LobbyConfig.Instance.GetLobbyMode(), mapName: _selectedMapFile.MapName);
    }

    public void ShowSavePanel()
    {
        loadPanel.SetActive(false);
        savePanel.SetActive(true);

        ShowMapFiles();
    }

    public void ShowLoadPanel()
    {
        loadPanel.SetActive(true);
        savePanel.SetActive(false);

        ShowMapFiles();
    }
}