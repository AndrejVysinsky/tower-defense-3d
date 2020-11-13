using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EntityPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI entityName;
    [SerializeField] Image entityImage;
    [SerializeField] HitPointsPanel hitPointsPanel;

    [SerializeField] TextMeshProUGUI damageValue;
    [SerializeField] TextMeshProUGUI armorValue;

    private void OnEnable()
    {
        var interactingObject = InteractionSystemButBetter.Instance.InteractingGameObject;

        if (interactingObject.TryGetComponent(out IEntity entity))
        {
            entityName.text = entity.Name;
            entityImage.sprite = entity.Sprite;

            hitPointsPanel.SetEntity(entity);
        }
        
        if (interactingObject.TryGetComponent(out IEntityDamage entityDamage))
        {
            damageValue.text = entityDamage.DamageValue.ToString();
        }

        if (interactingObject.TryGetComponent(out IEntityArmor entityArmor))
        {
            armorValue.text = entityArmor.ArmorValue.ToString();
        }
    }
}