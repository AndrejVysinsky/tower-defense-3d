using System.Collections.Generic;
using UnityEngine;

public class InteractionPanel : MonoBehaviour
{
    [SerializeField] GameObject upgradePrefab;

    private List<GameObject> _upgradePanels = new List<GameObject>();

    private void OnEnable()
    {
        var interactingObject = InteractionSystem.Instance.InteractingGameObject;

        if (interactingObject.TryGetComponent(out IUpgradable upgradable))
        {
            _upgradePanels.ForEach(upgrade => Destroy(upgrade));

            for (int i = 0; i < upgradable.UpgradeOptions.Count; i++)
            {
                var upgradeOption = upgradable.UpgradeOptions[i];

                var upgradePanel = Instantiate(upgradePrefab, transform);
                _upgradePanels.Add(upgradePanel);

                upgradePanel.GetComponent<UpgradePanel>().SetUpgrade(upgradable, upgradeOption, i);
                upgradePanel.GetComponent<TooltipTrigger>().SetTooltip(upgradeOption.Tooltip);
            }
        }
    }
}