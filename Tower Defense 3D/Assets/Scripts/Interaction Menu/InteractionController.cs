using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractionController : MonoBehaviour, IInteractionChanged
{
    [SerializeField] GameObject buyContainer;
    [SerializeField] GameObject optionContainer;
    [SerializeField] GameObject optionPrefab;

    private InteractionList _entityOptionMenu;

    private void OnEnable()
    {
        EventManager.AddListener(gameObject);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(gameObject);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (RayCaster.RayCastUIObject(out RaycastResult raycastResult))
                return;

            if (GridController.IsBuildingModeActive)
                return;

            HideInteractionMenu();

            if (RayCaster.RayCastGameObject(out RaycastHit hitInfo))
            {
                if (hitInfo.transform.gameObject.TryGetComponent(out InteractionList interactionMenu))
                {
                    ShowInteractionMenu(interactionMenu);
                }
            }
        }
    }

    private void ShowInteractionMenu(InteractionList entityOptionMenu)
    {
        optionContainer.SetActive(true);
        buyContainer.SetActive(false);

        _entityOptionMenu = entityOptionMenu;

        DestroyOldOptions();
        PopulateOptions();
    }

    private void HideInteractionMenu()
    {
        optionContainer.SetActive(false);
        buyContainer.SetActive(true);
    }

    public void OnInteractionHidden()
    {
        HideInteractionMenu();
    }

    public void OnInteractionRemoved(string interactionName, UnityAction action)
    {
        var listOfInteractions = _entityOptionMenu.Interactions.Select(x => x.InteractionActions).ToList();

        for (int i = 0; i < listOfInteractions.Count; i++)
        {
            var interactionsActions = listOfInteractions[i];

            for (int j = 0; j < interactionsActions.Count; j++)
            {
                var eventMethodName = interactionsActions[j].Method.Name;

                if (eventMethodName == interactionName)
                {
                    //remove listener
                    int before = _entityOptionMenu.Interactions[i].InteractionActions.Count;

                    _entityOptionMenu.Interactions[i].InteractionActions.RemoveAt(i);

                    int after = _entityOptionMenu.Interactions[i].InteractionActions.Count;

                    if (_entityOptionMenu.Interactions[i].InteractionActions.Count == 0)
                    {
                        _entityOptionMenu.Interactions.RemoveAt(i);
                        optionContainer.transform.GetChild(i).gameObject.SetActive(false);

                        break;
                    }
                }
            }
        }
    }

    private void DestroyOldOptions()
    {
        foreach (Transform child in optionContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void PopulateOptions()
    {
        var entityOptions = _entityOptionMenu.Interactions;

        entityOptions.ForEach(x =>
        {
            var option = Instantiate(optionPrefab, optionContainer.transform);

            option.GetComponent<Image>().sprite = x.InteractionSprite;

            x.InteractionActions.ForEach(action =>
            {
                option.GetComponent<Button>().onClick.AddListener(action);
            });

            if (x.InteractionActions.Count == 0)
            {
                option.SetActive(false);
            }
        });
    }

    public void OnInteractionAdded(UnityAction action)
    {
        throw new System.NotImplementedException();
    }
}