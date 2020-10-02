using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "Data/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [SerializeField] float speed;
    [SerializeField] float health;
    [SerializeField] int damageToPlayer;
    [SerializeField] int rewardToPlayer;

    public float Speed => speed;
    public float Health => health;
    public int DamageToPlayer => damageToPlayer;
    public int RewardToPlayer => rewardToPlayer;
}