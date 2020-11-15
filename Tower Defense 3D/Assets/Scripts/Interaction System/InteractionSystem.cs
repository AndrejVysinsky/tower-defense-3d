using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    public static InteractionSystem Instance { get; private set; }

    [SerializeField] GameObject buyContainer;

    [SerializeField] GameObject entityPanel;
    [SerializeField] GameObject interactionPanel;
    [SerializeField] GameObject interactionIndicator;

    public GameObject InteractingGameObject { get; private set; }

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
        if (InteractingGameObject == null)
        {
            HideInteractions();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (RayCaster.RayCastUIObject(out _))
                return;

            if (GridController.IsBuildingModeActive)
                return;

            HideInteractions();

            if (RayCaster.RayCastGameObject(out RaycastHit hitInfo))
            {
                if (hitInfo.transform.gameObject.TryGetComponent(out IInteractable _))
                {
                    InteractingGameObject = hitInfo.transform.gameObject;

                    ShowInteractions();
                }
            }
        }
    }

    private void ShowInteractions()
    {
        entityPanel.SetActive(true);
        interactionPanel.SetActive(true);
        interactionIndicator.SetActive(true);

        buyContainer.SetActive(false);
    }

    private void HideInteractions()
    {
        entityPanel.SetActive(false);
        interactionPanel.SetActive(false);
        interactionIndicator.SetActive(false);

        buyContainer.SetActive(true);
    }

    public void RefreshInteractions()
    {
        HideInteractions();
        ShowInteractions();
    }
}
