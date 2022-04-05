using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [ServerCallback]
    private void OnEnable()
    {
        waveTimer.OnTimerSkipped += OnWaveSkipped;

        EventManager.AddListener(gameObject);
    }

    [ServerCallback]
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

            GameController.Instance.WaveSpawned(WaveNumber++);

            yield return StartCoroutine(SpawnWave(_waveIndex++));
            yield return StartCoroutine(WaitForWaveDelay());

            if (WaveNumber % addMultiplierEveryXWaves == 0)
            {
                _difficultyMultiplier += difficultyMultiplierToAdd;
            }
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

        if (LobbyConfig.Instance.GetLobbyMode() == LobbyConfig.LobbyMode.Versus)
        {
            for (int i = 0; i < _pathways.Length; i++)
            {
                var player = FindObjectsOfType<NetworkPlayer>().FirstOrDefault(x => x.MyInfo.netId == _pathways[i].PlayerId);
                player.AddEnemiesToCreepCount(numberOfEnemies);
            }
        }
        else
        {
            var players = FindObjectsOfType<NetworkPlayer>().ToList();
            foreach (var player in players)
            {
                player.AddEnemiesToCreepCount(numberOfEnemies);
            }
        }
                
        while (numberOfEnemies > 0)
        {
            SpawnEnemy(index);
            numberOfEnemies--;

            yield return new WaitForSeconds(enemyWave.SpawnDelay);
        }

        //forced delay
        yield return new WaitForSeconds(2f);
    }

    [Server]
    private void SpawnEnemy(int waveIndex)
    {
        var enemyWave = enemyWaves[waveIndex];

        for (int i = 0; i < _pathways.Length; i++)
        {
            var enemyObject = Instantiate(enemyWave.EnemyType, transform);
            enemyObject.GetComponent<Enemy>().Initialize(_pathways[i], _difficultyMultiplier);

            NetworkServer.Spawn(enemyObject);
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
