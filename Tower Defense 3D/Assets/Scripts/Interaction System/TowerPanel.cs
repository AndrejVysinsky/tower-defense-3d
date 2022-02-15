using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Interaction_System
{
    public class TowerPanel : MonoBehaviour
    {
        [SerializeField] Image image;
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] TextMeshProUGUI damageText;
        [SerializeField] TextMeshProUGUI attackDelayText;
        [SerializeField] TextMeshProUGUI aoeText;
        [SerializeField] TextMeshProUGUI rangeText;

        [SerializeField] UpgradePanel upgradePanel;
        [SerializeField] SellPanel sellPanel;

        private TowerBase _towerBase;

        private void OnEnable()
        {
            var towerObject = InteractionSystem.Instance.InteractingGameObject;
            if (towerObject == null)
            {
                Debug.LogError("Tower object is null.");
                return;
            }

            if (towerObject.TryGetComponent(out TowerBase towerBase)) 
            {
                _towerBase = towerBase;
            }
            else
            {
                Debug.LogError("No tower base on object.");
                return;
            }

            ShowTowerRange(true);
            ShowTowerStats();
            ShowInteractions();
        }

        private void OnDisable()
        {
            ShowTowerRange(false);
        }

        private void ShowTowerRange(bool show)
        {

        }

        private void ShowTowerStats()
        {
            var towerData = _towerBase.TowerData;

            image.sprite = towerData.Sprite;
            nameText.text = towerData.Name;
            damageText.text = towerData.Damage.ToString();
            attackDelayText.text = towerData.AttackDelay.ToString();
            rangeText.text = towerData.Radius.ToString();
        }

        private void ShowInteractions()
        {
            if (_towerBase.UpgradeOptions.Count > 0)
            {
                upgradePanel.SetUpgrade(_towerBase.TowerData.NextUpgrades[0], 0);
            }
            else
            {
                upgradePanel.SetNoUpgradeAvailable();
            }

            sellPanel.SetTowerData(_towerBase);
        }
    }
}
