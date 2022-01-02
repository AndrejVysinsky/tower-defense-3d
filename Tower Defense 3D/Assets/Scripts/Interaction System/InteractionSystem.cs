using Mirror;
using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    public static InteractionSystem Instance { get; private set; }

    [SerializeField] GameObject towerPanel;
    [SerializeField] GameObject enemyPanel;
    [SerializeField] GameObject buyPanel;

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

        interactionIndicator.SetActive(true);
        
        if (hasAuthority)
        {
            if (InteractingGameObject.TryGetComponent(out TowerBase towerBase))
            {
                towerPanel.SetActive(true);
            }
        }

        if (InteractingGameObject.TryGetComponent(out Enemy enemy))
        {
            enemyPanel.SetActive(true);
        }

        buyPanel.SetActive(false);
    }

    private void HideInteractions()
    {
        interactionIndicator.SetActive(false);
        towerPanel.SetActive(false);
        enemyPanel.SetActive(false);

        buyPanel.SetActive(true);        
    }

    public void RefreshInteractions()
    {
        if (InteractingGameObject == null)
            return;

        HideInteractions();
        ShowInteractions();
    }
}
