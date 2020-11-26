using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Data", menuName = "Data/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [SerializeField] new string name;
    [SerializeField] Sprite sprite;
    [SerializeField] float speed;
    [SerializeField] float health;
    [SerializeField] int damageToPlayer;
    [SerializeField] int rewardToPlayer;

    public string Name => name;
    public Sprite Sprite => sprite;
    public float Speed => speed;
    public float Health => health;
    public int DamageToPlayer => damageToPlayer;
    public int RewardToPlayer => rewardToPlayer;
}