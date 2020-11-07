using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ObjectInteractions;

public class InteractionSystem : MonoBehaviour
{
    public static InteractionSystem Instance { get; private set; }

    [SerializeField] GameObject interactionPrefab;
    [SerializeField] GameObject interactionContainer;
    [SerializeField] GameObject buyContainer;

    private GameObject _interactingGameObject;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (RayCaster.RayCastUIObject(out RaycastResult raycastResult))
                return;

            if (GridController.IsBuildingModeActive)
                return;

            HideInteractions();

            if (RayCaster.RayCastGameObject(out RaycastHit hitInfo))
            {
                if (hitInfo.transform.gameObject.TryGetComponent(out ObjectInteractions interactions))
                {
                    _interactingGameObject = hitInfo.transform.gameObject;

                    ShowInteractions(interactions.Interactions);
                }
            }
        }
    }

    public void ShowInteractions(List<Interaction> myInteractions)
    {
        interactionContainer.SetActive(true);
        buyContainer.SetActive(false);

        foreach (Transform child in interactionContainer.transform)
        {
            Destroy(child.gameObject);
        }

        myInteractions.ForEach(interaction =>
        {
            var action = Instantiate(interactionPrefab, interactionContainer.transform);

            action.GetComponent<InteractionTrigger>().SetAction(interaction.UnityAction);

            if (interaction.TooltipEnabled)
            {
                interaction.Tooltip.ObjectData = _interactingGameObject;
                action.GetComponent<TooltipTrigger>().SetTooltip(interaction.Tooltip);
            }
            
            action.GetComponent<Image>().sprite = interaction.InteractionSprite;
        });
    }

    public void HideInteractions()
    {
        interactionContainer.SetActive(false);
        buyContainer.SetActive(true);
    }
}
