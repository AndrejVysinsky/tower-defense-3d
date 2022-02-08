using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapExplorer : MonoBehaviour
{
    [SerializeField] GameObject mapCardPrefab;
    [SerializeField] GameObject mapCardContainer;
    
    private string[] defaultMaps;
    private string[] defaultMapsSprites;
    private string[] customMaps;

    private List<MapCard> mapCards;

    public MapCard SelectedMapCard { get; private set; } = null;

    private void Start()
    {
        mapCards = new List<MapCard>();

        defaultMaps = FileManager.DefaultMaps;
        defaultMapsSprites = FileManager.DefaultMapsSprites;
        customMaps = FileManager.GetFiles(FileManager.MapPath);

        ShowMapCards();
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

        for (int i = 0; i < customMaps.Length; i++)
        {
            var mapCardObject = Instantiate(mapCardPrefab, mapCardContainer.transform);
            var mapCard = mapCardObject.GetComponent<MapCard>();

            mapCard.Initialize(this, true, customMaps[i]);
            mapCards.Add(mapCard);
        }

        SelectMapCard(mapCards[0]);
    }

    public void SelectMapCard(MapCard mapCard)
    {
        SelectedMapCard?.DeselectCard();
        
        SelectedMapCard = mapCard;

        SelectedMapCard.SelectCard();
    }

}