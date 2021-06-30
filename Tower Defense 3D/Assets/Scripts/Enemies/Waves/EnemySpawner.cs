using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : NetworkBehaviour, IMapLoaded
{
    [Header("Settings")]
    [SerializeField] WaveTimer waveTimer;
    [SerializeField] float waveDelay;

    [Range(0, 5)] [SerializeField] float difficultyMultiplierToAdd;
    [SerializeField] int addMultiplierEveryXWaves;

    [SerializeField] List<EnemyWaveData> enemyWaves;

    private Pathway[] _pathways;

    private int _waveIndex = 0;
    private float _difficultyMultiplier = 0;

    private bool _waveSkipped = false;

    public int WaveNumber { get; private set; } = 1;
    public int WaveCount => enemyWaves.Count;

    [Server]
    private void OnEnable()
    {
        waveTimer.OnTimerSkipped += OnWaveSkipped;

        EventManager.AddListener(gameObject);
    }

    [Server]
    private void OnDisable()
    {
        waveTimer.OnTimerSkipped -= OnWaveSkipped;

        EventManager.RemoveListener(gameObject);
    }

    [Server]
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

            GameController.Instance.WaveSpawned(WaveNumber++);

            yield return StartCoroutine(SpawnWave(_waveIndex++));

            yield return StartCoroutine(WaitForWaveDelay());
        }
    }

    [Server]
    IEnumerator WaitForWaveDelay()
    {
        waveTimer.RefreshTimer(waveDelay);

        while (waveTimer.RemainingTime > 0)
        {
            if (_waveSkipped)
                break;

            yield return new WaitForSeconds(1f);
        }

        //forced delay
        yield return new WaitForSeconds(2f);
        
        _waveSkipped = false;
    }

    [Server]
    IEnumerator SpawnWave(int index)
    {
        var enemyWave = enemyWaves[index];

        int numberOfEnemies = enemyWave.NumberOfEnemies;

        var enemyType = enemyWave.EnemyType;
        //var enemySprite = enemyWave.GetRandomSprite();
        //var enemyColor = enemyWave.GetRandomColor();        

        while (numberOfEnemies > 0)
        {
            RpcSpawnEnemy(index);
            numberOfEnemies--;

            yield return new WaitForSeconds(enemyWave.SpawnDelay);
        }

        //forced delay
        yield return new WaitForSeconds(2f);
    }

    [ClientRpc]
    private void RpcSpawnEnemy(int waveIndex)
    {
        var enemyWave = enemyWaves[waveIndex];

        Debug.Log(waveIndex);

        for (int i = 0; i < _pathways.Length; i++)
        {
            var enemyObject = Instantiate(enemyWave.EnemyType, transform);

            enemyObject.GetComponent<Enemy>().Initialize(_pathways[i], _difficultyMultiplier);
        }
    }

    private void OnWaveSkipped()
    {
        _waveSkipped = true;
    }

    [Server]
    public void OnMapBeingLoaded(MapSaveData mapSaveData, bool isLoadingInEditor)
    {
        RpcFindPathways();
        StartCoroutine(Spawner());
    }

    [ClientRpc]
    private void RpcFindPathways()
    {
        _pathways = FindObjectsOfType<Pathway>();
    }
}
