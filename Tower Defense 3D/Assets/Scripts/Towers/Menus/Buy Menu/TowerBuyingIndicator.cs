using UnityEngine;

public class TowerBuyingIndicator
{
    private readonly GameObject _towerIndicator;
    private readonly GameObject _towerPlacementIndicator;
    private readonly GameObject _touchIndicator;

    private readonly float _touchIndicatorSpeed = 25f;

    public TowerBuyingIndicator(GameObject towerIndicator, GameObject towerPlacementIndicator, GameObject touchIndicator)
    {
        _towerIndicator = towerIndicator;
        _towerPlacementIndicator = towerPlacementIndicator;
        _touchIndicator = touchIndicator;
    }

    private void SetActive(bool value)
    {
        _towerIndicator.SetActive(value);
        _towerPlacementIndicator.SetActive(value);
        _touchIndicator.SetActive(value);
    }

    private Vector3 GetUpdatedPosition(Vector3 newPosition)
    {
        //only for changing X and Y coordinates while preserving original Z

        var result = _towerIndicator.transform.position;

        result.x = newPosition.x;
        result.y = newPosition.y;

        return result;
    }

    public void ShowIndicators(Vector3 position)
    {
        SetActive(true);

        position = GetUpdatedPosition(position);

        _towerIndicator.transform.position = position;
        _towerPlacementIndicator.transform.position = position;
        _touchIndicator.transform.position = position;
    }

    public void HideIndicators()
    {
        SetActive(false);
    }

    public bool AreIndicatorsActive()
    {
        return _touchIndicator.activeSelf;
    }

    public void MoveTowerIndicators(Vector3 position)
    {
        position = GetUpdatedPosition(position);

        _towerIndicator.transform.position = position;
        _towerPlacementIndicator.transform.position = position;
    }

    public void MoveTouchIndicator(Vector3 destination, float deltaTime)
    {
        _touchIndicator.transform.position = Vector3.MoveTowards(_touchIndicator.transform.position, destination, _touchIndicatorSpeed * deltaTime);
    }

    public Vector3 GetTowerPosition()
    {
        return _towerIndicator.transform.position;
    }
}