using System.Collections.Generic;
using UnityEngine;

public class InteractionPanel : MonoBehaviour
{
    [SerializeField] GameObject upgradePrefab;
    [SerializeField] GameObject sellPrefab;

    private List<GameObject> _interactions = new List<GameObject>();

    private void OnEnable()
    {
        ShowInteractions();
    }

    private void ShowInteractions()
    {
        var interactingObject = InteractionSystem.Instance.InteractingGameObject;

        if (interactingObject == null)
            return;

        _interactions.ForEach(interaction => Destroy(interaction));

        ShowUpgradeOptions(interactingObject);
        ShowSellOption(interactingObject);
    }

    private void ShowUpgradeOptions(GameObject interactingObject)
    {
        if (interactingObject.TryGetComponent(out IUpgradeable upgradable))
        {
            for (int i = 0; i < upgradable.UpgradeOptions.Count; i++)
            {
                var upgradeOption = upgradable.UpgradeOptions[i];

                var upgradePanel = Instantiate(upgradePrefab, transform);
                _interactions.Add(upgradePanel);

                upgradePanel.GetComponent<UpgradePanel>().SetUpgrade(upgradable, upgradeOption);
                upgradePanel.GetComponent<TooltipTrigger>().SetTooltip(upgradeOption.Tooltip);
            }
        }
    }

    private void ShowSellOption(GameObject interactingObject)
    {
        if (interactingObject.TryGetComponent(out ISellable sellable))
        {
            var sellPanel = Instantiate(sellPrefab, transform);
            _interactions.Add(sellPanel);

            sellPanel.GetComponent<SellPanel>().SetSell(sellable);
            sellPanel.GetComponent<TooltipTrigger>().SetTooltip(sellable.Tooltip);
        }
    }

    public void OnPanelClicked()
    {
        ShowInteractions();
    }
}