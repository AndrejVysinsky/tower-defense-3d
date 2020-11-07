using System;
using UnityEngine;

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
