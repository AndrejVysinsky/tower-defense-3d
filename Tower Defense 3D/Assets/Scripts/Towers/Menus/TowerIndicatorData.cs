using UnityEngine;

[CreateAssetMenu(fileName = "Tower Indicator Data", menuName = "Data/Tower Indicator Data")]
public class TowerIndicatorData : ScriptableObject
{
    [SerializeField] GameObject towerIndicator;
    [SerializeField] GameObject towerPlacementIndicator;
    [SerializeField] GameObject touchInputIndicator;

    public GameObject TowerIndicator => towerIndicator;
    public GameObject TowerPlacementIndicator => towerPlacementIndicator;
    public GameObject TouchInputIndicator => touchInputIndicator;
}
