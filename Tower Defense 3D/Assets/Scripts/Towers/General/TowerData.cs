using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tower Data", menuName = "Data/Tower Data")]
public class TowerData : ScriptableObject
{
    [SerializeField] float sellFactor;

    [SerializeField] List<TowerLevelData> levelData;

    public float SellFactor => sellFactor;
    public int MaxLevel => levelData.Count;

    public TowerLevelData GetLevelData(int level)
    {
        return levelData[level - 1];
    }

    [Serializable]
    public class TowerLevelData
    {
        [SerializeField] int price;
        [SerializeField] float damage;
        [SerializeField] float attackDelay;
        [SerializeField] float radius;
        [SerializeField] Material material;

        public int Price => price;
        public float Damage => damage;
        public float AttackDelay => attackDelay;
        public float Radius => radius;
        public Material Material => material;
    }
}
