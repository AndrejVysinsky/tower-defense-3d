using TMPro;
using UnityEngine;

public class HitPointsPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currentHitPointsText;
    [SerializeField] TextMeshProUGUI totalHitPointsText;

    private IEntity _entity;

    public void SetEntity(IEntity entity)
    {
        _entity = entity;
    }

    private void Update()
    {
        if (_entity == null)
            return;

        currentHitPointsText.text = _entity.CurrentHitPoints.ToString();
        totalHitPointsText.text = _entity.TotalHitPoints.ToString();
    }
}
