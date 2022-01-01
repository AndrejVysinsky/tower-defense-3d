using Mirror;
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
        bool hasAuthority = false;
        if (InteractingGameObject.TryGetComponent(out NetworkIdentity networkIdentity))
        {
            if (networkIdentity.hasAuthority)
            {
                hasAuthority = true;
            }
        }
        else
        {
            hasAuthority = true;
        }

        entityPanel.SetActive(true);
        interactionIndicator.SetActive(true);
        
        if (hasAuthority)
        {
            interactionPanel.SetActive(true);
        }

        buyContainer.SetActive(false);
    }

    private void HideInteractions()
    {
        entityPanel.SetActive(false);
        interactionIndicator.SetActive(false);
        interactionPanel.SetActive(false);

        buyContainer.SetActive(true);        
    }

    public void RefreshInteractions()
    {
        if (InteractingGameObject == null)
            return;

        HideInteractions();
        ShowInteractions();
    }
}
