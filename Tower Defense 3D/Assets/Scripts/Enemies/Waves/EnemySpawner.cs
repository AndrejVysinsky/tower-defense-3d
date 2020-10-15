using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] List<Path> paths;
    [SerializeField] WaveTimer waveTimer;

    [SerializeField] float waveDelay;

    [Range(0, 5)] [SerializeField] float difficultyMultiplierToAdd;
    [SerializeField] int addMultiplierEveryXWaves;

    [SerializeField] List<EnemyWaveData> enemyWaves;

    private int _waveIndex = 0;
    private float _difficultyMultiplier = 0;

    private bool _waveSkipped = false;

    private Path _randomPath;

    public int WaveNumber { get; private set; } = 1;
    public int WaveCount => enemyWaves.Count;

    void Start()
    {
        if (waveTimer == null)
        {
            waveTimer = paths[0].PortalStart.GetTimer();
        }

        StartCoroutine(Spawner());
    }

    private void OnEnable()
    {
        if (waveTimer == null)
        {
            waveTimer = paths[0].PortalStart.GetTimer();
        }

        waveTimer.OnTimerSkipped += OnWaveSkipped;
    }

    private void OnDisable()
    {
        waveTimer.OnTimerSkipped -= OnWaveSkipped;
    }

    IEnumerator Spawner()
    {
        yield return StartCoroutine(WaitForWaveDelay());

        while (true)
        {
            if (_waveIndex >= enemyWaves.Count)
            {
                _waveIndex = 0;
            }

            if (WaveNumber % addMultiplierEveryXWaves == 0)
            {
                _difficultyMultiplier += difficultyMultiplierToAdd;
            }

            //GameController.Instance.WaveSpawned(WaveNumber++);

            yield return StartCoroutine(SpawnWave(_waveIndex++));

            yield return StartCoroutine(WaitForWaveDelay());
        }
    }

    IEnumerator WaitForWaveDelay()
    {
        int waitCount = 0;

        waveTimer.RefreshTimer(waveDelay);

        while (waitCount < waveDelay)
        {
            if (_waveSkipped)
                break;

            waitCount++;
            yield return new WaitForSeconds(1f);
        }

        //forced delay
        yield return new WaitForSeconds(2f);
        
        _waveSkipped = false;
    }

    IEnumerator SpawnWave(int index)
    {
        var enemyWave = enemyWaves[index];

        int numberOfEnemies = enemyWave.NumberOfEnemies;

        var enemyType = enemyWave.EnemyType;
        var enemySprite = enemyWave.GetRandomSprite();
        var enemyColor = enemyWave.GetRandomColor();

        _randomPath = paths[Random.Range(0, paths.Count)];

        while (numberOfEnemies > 0)
        {
            SpawnEnemy(enemyType, enemySprite, enemyColor);
            numberOfEnemies--;

            yield return new WaitForSeconds(enemyWave.SpawnDelay);
        }

        //forced delay
        yield return new WaitForSeconds(2f);
    }

    private void SpawnEnemy(GameObject enemyPrefab, Sprite enemySprite, Color enemyColor)
    {
        var enemyObject = Instantiate(enemyPrefab, transform);

        //var enemySpriteRenderer = enemyObject.GetComponent<SpriteRenderer>();

        //enemySpriteRenderer.sprite = enemySprite;
        //enemySpriteRenderer.color = enemyColor;

        enemyObject.GetComponent<Enemy>().Initialize(_randomPath, enemySprite, enemyColor, _difficultyMultiplier);
    }

    public void OnWaveSkipped()
    {
        _waveSkipped = true;
    }
}
