using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapCard : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] Image cardBackground;
    [SerializeField] Image mapImage;
    [SerializeField] TextMeshProUGUI nameText;

    [Header("Colors")]
    [SerializeField] Color defaultColor;
    [SerializeField] Color selectedColor;

    public string MapName { get; private set; }
    public bool IsCustomMap { get; private set; }

    private MapExplorer _mapExplorer;

    public void Initialize(MapExplorer mapExplorer, bool isCustomMap, string mapName, Sprite mapSprite = null)
    {
        _mapExplorer = mapExplorer;
        IsCustomMap = isCustomMap;
        MapName = mapName;
        nameText.text = mapName;

        if (mapSprite != null)
            mapImage.sprite = mapSprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _mapExplorer.SelectMapCard(this);
    }

    public void SelectCard()
    {
        cardBackground.color = selectedColor;
    }

    public void DeselectCard() 
    {
        cardBackground.color = defaultColor;
    }
}