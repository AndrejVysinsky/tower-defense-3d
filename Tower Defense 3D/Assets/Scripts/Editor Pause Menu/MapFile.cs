using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapFile : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] TextMeshProUGUI mapName;

    public string MapName { get; set; }

    private MapFileExplorer _mapFileExplorer;

    public void PopulateThumbnailData(string name, MapFileExplorer mapFileExplorer)
    {
        _mapFileExplorer = mapFileExplorer;
        MapName = name;
        mapName.text = name;
    }

    public void SelectFile()
    {
        mapName.text = "selected";
    }

    public void DeselectFile()
    {
        mapName.text = MapName;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _mapFileExplorer.MapFileSelected(this);
        SelectFile();
    }
}