using Assets.Scripts.Network;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] int startingCurrency;
    [SerializeField] int startingLives;

    public readonly SyncList<uint> connectedPlayers = new SyncList<uint>();

    private List<NetworkConnection> _networkConnections = new List<NetworkConnection>();
    private List<GameObject> _spawnPrefabs;

    public Boundaries PlayerBoundaries { get; set; }

    private Boundaries _mapBoundaries;
    private CameraController _cameraController;
    private GridController _gridController;

    [SyncVar] private int _currency;
    [SyncVar] private int _lives;
    public int Currency => _currency;
    public int Lives => _lives;

    public uint PlayerId { get; private set; }

    private void Awake()
    {
        _cameraController = FindObjectOfType<CameraController>();
        _gridController = FindObjectOfType<GridController>();
        _spawnPrefabs = FindObjectOfType<NetworkManager>().spawnPrefabs;

        _currency = startingCurrency;
        _lives = startingLives;
    }

    private void Start()
    {
        PlayerId = GetComponent<NetworkIdentity>().netId;

        EventManager.ExecuteEvent<IServerEvents>((x, y) => x.OnPlayerInitialized());
    }

    public void SetMapBoundaries(Boundaries mapBoundaries)
    {
        if (mapBoundaries == null)
            return;

        _mapBoundaries = mapBoundaries;

        if (isLocalPlayer)
        {
            _cameraController.SetCameraBoundaries(_mapBoundaries);
            _cameraController.SetCameraPosition(PlayerBoundaries.GetMiddlePoint().x, PlayerBoundaries.GetMiddlePoint().z);

            _gridController.Boundaries = PlayerBoundaries;
        }
    }

    [Server]
    public void PlayerConnected(uint playerId)
    {
        connectedPlayers.Add(playerId);
        Debug.Log("connected with id " + playerId);
    }

    [Server]
    public void PlayerDisconnected(uint playerId)
    {
        connectedPlayers.Remove(playerId);
    }

    public SyncList<uint> GetPlayerConnections()
    {
        return connectedPlayers;
    }

    public void Spawn(int spawnableIndex, Vector3 pos, Quaternion rotation)
    {
        CmdSpawnTower(spawnableIndex, pos, rotation, PlayerId);
    }

    [Command]
    private void CmdSpawnTower(int spawnableIndex, Vector3 pos, Quaternion rotation, uint playersNetId)
    {
        //has enough currency
        var price = _spawnPrefabs[spawnableIndex].GetComponent<TowerBase>().TowerData.Price;
        if (Currency < price)
        {
            Debug.Log("Not enough currency!");
            return;
        }

        var playerGameObject = FindObjectsOfType<NetworkIdentity>().FirstOrDefault(x => x.netId == playersNetId).gameObject;

        var towerObject = Instantiate(_spawnPrefabs[spawnableIndex], pos, rotation);
        towerObject.GetComponent<TowerBase>().SetPlayerId(playersNetId);

        NetworkServer.Spawn(towerObject, playerGameObject);

        RpcUpgradeTower(towerObject.GetComponent<NetworkIdentity>().netId);
        UpdateCurrency(playersNetId, -price);
    }

    [ClientRpc]
    public void RpcUpgradeTower(uint towerNetId)
    {
        var towers = FindObjectsOfType<TowerBase>();

        for (int i = 0; i < towers.Length; i++)
        {
            if (towers[i].TryGetComponent(out NetworkIdentity networkIdentity))
            {
                if (networkIdentity.netId == towerNetId)
                {
                    var upgradeable = towers[i].GetComponent<IUpgradeable>();
                    towers[i].OnUpgradeStarted(upgradeable.CurrentUpgrade, out bool upgradeStarted);
                }
            }
        }
    }

    [Server]
    public void UpdateCurrency(uint playersNetId, int value)
    {
        _currency += value;

        RpcUpdateCurrency(playersNetId, _currency);
    }

    [ClientRpc]
    public void RpcUpdateCurrency(uint playersNetId, int currency)
    {
        Debug.Log("Updating currency for clients");

        EventManager.ExecuteEvent<IPlayerEvents>((x, y) => x.OnCurrencyUpdated(playersNetId, currency));
    }

    [Server]
    public void UpdateLives(uint playersNetId, int value)
    {
        _lives += value;

        RpcUpdateLives(playersNetId, _lives);
    }

    [ClientRpc]
    public void RpcUpdateLives(uint playersNetId, int lives)
    {
        Debug.Log("Updating lives for clients");

        EventManager.ExecuteEvent<IPlayerEvents>((x, y) => x.OnLivesUpdated(playersNetId, lives));
    }
}
