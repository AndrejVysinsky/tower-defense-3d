using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDummy : MonoBehaviour
{
    [SerializeField] TowerData towerData;

    private RangeRenderer _rangeRenderer;

    void Start()
    {
        var lineRenderer = GetComponent<LineRenderer>();
        _rangeRenderer = new RangeRenderer(lineRenderer, towerData.GetLevelData(1).Radius);
        _rangeRenderer.ShowRange();
    }
}
