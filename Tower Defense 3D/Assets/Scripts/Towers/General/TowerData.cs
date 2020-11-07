using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tower Data", menuName = "Data/Tower Data")]
public class TowerData : ScriptableObject
{
    [SerializeField] float sellFactor;
    [SerializeField] List<TowerLevelData> levelData;

    public int MaxLevel => levelData.Count;

    public TowerLevelData GetLevelData(int level)
    {
        return levelData[level - 1];
    }

    public int GetSellValue(int level)
    {
        return (int)(GetLevelData(level).Price * sellFactor);
    }
}
