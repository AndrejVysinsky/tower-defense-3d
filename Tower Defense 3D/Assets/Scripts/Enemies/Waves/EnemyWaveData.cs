using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Wave Data", menuName = "Data/Enemy Wave Data")]
public class EnemyWaveData : ScriptableObject
{
    [SerializeField] GameObject enemyType;
    [SerializeField] int numberOfEnemies;
    [SerializeField] float spawnDelay;

    [SerializeField] List<Sprite> spriteVariations;
    [SerializeField] List<Color> colorVariations;

    public GameObject EnemyType => enemyType;
    public int NumberOfEnemies => numberOfEnemies;
    public float SpawnDelay => spawnDelay;

    public Sprite GetRandomSprite()
    {
        return spriteVariations[Random.Range(0, spriteVariations.Count)];
    }

    public Color GetRandomColor()
    {
        return colorVariations[Random.Range(0, colorVariations.Count)];
    }
}
