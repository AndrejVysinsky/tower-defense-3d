using TMPro;
using UnityEngine;

public class MapFileExplorer : MonoBehaviour
{
    [SerializeField] Transform mapFileExplorerWindow;
    [SerializeField] GameObject mapFilePrefab;
    [SerializeField] MapSaveManager mapManager;

    [SerializeField] TMP_InputField mapFileNameInput;

    private string[] _mapNames;
    private MapFile _selectedMapFile;

    public void ShowMapFiles()
    {
        _mapNames = mapManager.GetAllMaps();

        while (_mapNames.Length > transform.childCount)
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
        mapFileNameInput.text = mapFile.name;
    }

    public void SaveMapFile()
    {
        if (mapFileNameInput.text == null)
        {
            Debug.Log("Missing map name.");
            return;
        }

        mapManager.SaveMapData(mapFileNameInput.text);
    }
}