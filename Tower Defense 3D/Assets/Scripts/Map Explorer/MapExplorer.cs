using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapExplorer : MonoBehaviour
{
    [SerializeField] GameObject mapCardPrefab;
    [SerializeField] GameObject mapCardContainer;

    [Header("Editor only")]
    [SerializeField] EditorPauseMenu editorMenu;
    [SerializeField] MapSaveManager mapSaveManager;
    [SerializeField] TMP_InputField mapFileNameInput;
    [SerializeField] GameObject loadPanel;
    [SerializeField] GameObject savePanel;
    
    private string[] defaultMaps;
    private string[] defaultMapsSprites;
    private string[] customMaps;

    private List<MapCard> mapCards;

    public static bool mapExplorerInputActive = false;

    public MapCard SelectedMapCard { get; private set; } = null;

    private void Start()
    {
        mapCards = new List<MapCard>();

        defaultMaps = FileManager.DefaultMaps;
        defaultMapsSprites = FileManager.DefaultMapsSprites;

        ClearMapCards();
        ShowMapCards();
    }

    private void OnEnable()
    {
        mapExplorerInputActive = true;
    }

    private void OnDisable()
    {
        mapExplorerInputActive = false;
    }

    private void ShowMapCards()
    {
        //do not show default maps in editor
        if (SceneManager.GetActiveScene().name != "Editor Scene")
        {
            for (int i = 0; i < defaultMaps.Length; i++)
            {
                var mapCardObject = Instantiate(mapCardPrefab, mapCardContainer.transform);
                var mapCard = mapCardObject.GetComponent<MapCard>();
                var mapSprite = Resources.Load<Sprite>(FileManager.ResourceMapPath + defaultMapsSprites[i]);

                mapCard.Initialize(this, false, defaultMaps[i], mapSprite);
                mapCards.Add(mapCard);
            }
        }

        customMaps = FileManager.GetFiles(FileManager.MapPath);
        for (int i = 0; i < customMaps.Length; i++)
        {
            var mapCardObject = Instantiate(mapCardPrefab, mapCardContainer.transform);
            var mapCard = mapCardObject.GetComponent<MapCard>();

            mapCard.Initialize(this, true, customMaps[i]);
            mapCards.Add(mapCard);
        }

        SelectMapCard(mapCards[0]);
    }

    private void ClearMapCards()
    {
        for (int i = 0; i < mapCards.Count; i++)
        {
            Destroy(mapCards[i].gameObject);
        }
        mapCards.Clear();
        SelectedMapCard = null;
    }

    public void SelectMapCard(MapCard mapCard)
    {
        SelectedMapCard?.DeselectCard();
        
        SelectedMapCard = mapCard;

        SelectedMapCard.SelectCard();
    }

    public void ShowSavePanel()
    {
        loadPanel.SetActive(false);
        savePanel.SetActive(true);
    }

    public void ShowLoadPanel()
    {
        loadPanel.SetActive(true);
        savePanel.SetActive(false);
    }

    public void SaveMapFile()
    {
        if (string.IsNullOrEmpty(mapFileNameInput.text))
        {
            Debug.Log("Missing map name.");
            return;
        }

        mapSaveManager.SaveMapData(mapFileNameInput.text);

        ClearMapCards();
        ShowMapCards();
    }

    public void LoadMapFile(bool isLoadingInEditor)
    {
        if (SelectedMapCard == null)
        {
            Debug.Log("Missing map name.");
            return;
        }

        mapSaveManager.LoadMapData(isLoadingInEditor, null, mapName: SelectedMapCard.MapName);

        editorMenu.TogglePauseMenu();
    }

    public void RemoveMapFile(MapCard mapCard)
    {
        var success = FileManager.RemoveFile(FileManager.MapPath, mapCard.MapName);
        if (success)
        {
            ClearMapCards();
            ShowMapCards();
        }
    }
}