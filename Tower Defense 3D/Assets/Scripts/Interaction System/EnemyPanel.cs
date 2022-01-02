using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Interaction_System
{
    public class EnemyPanel : MonoBehaviour
    {
        [SerializeField] Image image;
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] TextMeshProUGUI damageText;
        [SerializeField] TextMeshProUGUI speedText;
        [SerializeField] TextMeshProUGUI rewardText;
        [SerializeField] TextMeshProUGUI hitPointsText;

        private Enemy _enemy;

        private void OnEnable()
        {
            var enemyObject = InteractionSystem.Instance.InteractingGameObject;
            if (enemyObject == null)
            {
                Debug.LogError("Enemy object is null.");
                return;
            }

            if (enemyObject.TryGetComponent(out Enemy enemy))
            {
                _enemy = enemy;
            }
            else
            {
                Debug.LogError("No enemy on object.");
                return;
            }

            ShowEnemyStats();
        }

        private void Update()
        {
            if (_enemy == null)
            {
                return;
            }
            UpdateEnemyHealth();
        }

        private void ShowEnemyStats()
        {
            var enemyData = _enemy.EnemyData;

            image.sprite = enemyData.Sprite;
            nameText.text = enemyData.Name;
            damageText.text = enemyData.DamageToPlayer.ToString();
            speedText.text = enemyData.Speed.ToString();
            rewardText.text = _enemy.ScaledReward.ToString();

            UpdateEnemyHealth();
        }

        private void UpdateEnemyHealth()
        {
            var currentHitPoints = _enemy.CurrentHitPoints.ToString();
            var totalHitPoints = _enemy.TotalHitPoints.ToString();

            hitPointsText.text = currentHitPoints + " / " + totalHitPoints;
        }
    }
}
