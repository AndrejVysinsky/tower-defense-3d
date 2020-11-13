using System.Collections.Generic;
using UnityEngine;

public class InteractionPanel : MonoBehaviour
{
    [SerializeField] GameObject upgradePrefab;

    private List<GameObject> _upgradePanels = new List<GameObject>();

    private void OnEnable()
    {
        var interactingObject = InteractionSystemButBetter.Instance.InteractingGameObject;

        if (interactingObject.TryGetComponent(out IUpgradable upgradable))
        {
            _upgradePanels.ForEach(upgrade => Destroy(upgrade));

            upgradable.UpgradeOptions.ForEach(upgradeOption =>
            {
                var upgradePanel = Instantiate(upgradePrefab, transform);

                upgradePanel.GetComponent<UpgradePanel>().SetUpgrade(upgradable, upgradeOption);

                _upgradePanels.Add(upgradePanel);
            });
        }
    }
}