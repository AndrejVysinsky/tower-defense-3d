using Assets.Scripts.Network;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [SerializeField] int startingCurrency;
    [SerializeField] int startingLives;
    [SerializeField] Sprite playerImage;

    public readonly SyncList<BasePlayerInfo> _playerInfoList = new SyncList<BasePlayerInfo>();

    private List<GameObject> _spawnPrefabs;

    public Boundaries PlayerBoundaries { get; set; }
    private Boundaries _mapBoundaries;
    private CameraController _cameraController;
    private GridController _gridController;

    private MyNetworkManager _myNetworkManager;

    [SyncVar] private int _currency;
    [SyncVar] private int _lives;
    [SyncVar] private int _myInfoIndex = -1;
    [SyncVar] private int _creeps = 0;
    public int Currency => _currency;
    public int Lives => _lives;
    public BasePlayerInfo MyInfo => _playerInfoList[_myInfoIndex];
    public Sprite PlayerImage => playerImage;

    private void Awake()
    {
        _myNetworkManager = FindObjectOfType<MyNetworkManager>();
        _cameraController = FindObjectOfType<CameraController>();
        _gridController = FindObjectOfType<GridController>();
        _spawnPrefabs = FindObjectOfType<NetworkManager>().spawnPrefabs;

        _currency = startingCurrency;
        _lives = startingLives;
    }

    private void Start()
    {
        EventManager.ExecuteEvent<IServerEvents>((x, y) => x.OnPlayerInitialized(this));
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
    public void PlayerConnected(uint netId, ulong steamId, string name, Color color)
    {
        var playerInfo = new BasePlayerInfo
        {
            netId = netId,
            steamId = steamId,
            name = name,
            color = color
        };
        _playerInfoList.Add(playerInfo);

        if (GetComponent<NetworkIdentity>().netId == netId)
        {
            _myInfoIndex = _playerInfoList.Count - 1;
        }
    }

    public void ChangeLobbyStatus(LobbyConfig.LobbyStatus lobbyStatus)
    {
        LobbyConfig.Instance.SetLobbyStatus(lobbyStatus);
    }

    public override void OnStopClient()
    {
        if (isLocalPlayer == false)
            return;

        if (LobbyConfig.Instance.GetLobbyStatus() == LobbyConfig.LobbyStatus.SceneChange)
            return;

        if (LobbyConfig.Instance.GetLobbyStatus() != LobbyConfig.LobbyStatus.Left)
        {
            var myLobbyManager = FindObjectOfType<MyLobbyManager>();

            if (myLobbyManager != null) {
                myLobbyManager.DisableCallbacks();
                Destroy(myLobbyManager);
            }

            var sceneLoader = FindObjectOfType<SceneLoader>();
            sceneLoader.ChangeScene(0);
        }
    }

    [Server]
    public void PlayerDisconnected(uint playerId)
    {
        _playerInfoList.RemoveAll(x => x.netId == playerId);

        RpcPlayerDisconnected(playerId);
    }

    [ClientRpc]
    private void RpcPlayerDisconnected(uint playerId)
    {
        if (isLocalPlayer == false)
            return;

        EventManager.ExecuteEvent<IServerEvents>((x, y) => x.OnPlayerDisconnected(playerId));
    }

    public List<uint> GetPlayerConnections()
    {
        return _playerInfoList.Select(x => x.netId).ToList();
    }

    public NetworkPlayer GetNetworkPlayer(uint netId)
    {
        return FindObjectsOfType<NetworkIdentity>().FirstOrDefault(x => x.netId == netId).GetComponent<NetworkPlayer>();
    }

    public void Spawn(int spawnableIndex, Vector3 pos, Quaternion rotation)
    {
        if (isLocalPlayer == false)
        {
            return;
        }

        CmdSpawnTower(spawnableIndex, pos, rotation, MyInfo.netId);
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

        RpcUpgradeTower(towerObject.GetComponent<NetworkIdentity>().netId, -1);
        UpdateCurrency(playersNetId, -price, towerObject.GetComponent<TowerBase>().GetFloatTextPosition());
    }

    public void UpgradeTower(TowerBase towerBase, int upgradePrice, int upgradeIndex)
    {
        if (towerBase.GetComponent<NetworkIdentity>().hasAuthority == false)
        {
            return;
        }
        
        CmdUpgradeTower(towerBase.GetComponent<NetworkIdentity>().netId, upgradePrice, upgradeIndex, towerBase.GetFloatTextPosition(), MyInfo.netId);
    }

    [Command]
    public void CmdUpgradeTower(uint towerNetId, int upgradePrice, int upgradeIndex, Vector3 upgradePosition, uint playersNetId)
    {
        //has enough currency
        if (Currency < upgradePrice)
        {
            Debug.Log("Not enough currency!");
            return;
        }

        RpcUpgradeTower(towerNetId, upgradeIndex);
        UpdateCurrency(playersNetId, -upgradePrice, upgradePosition);
    }

    [ClientRpc]
    public void RpcUpgradeTower(uint towerNetId, int upgradeIndex)
    {
        var towers = FindObjectsOfType<TowerBase>();

        for (int i = 0; i < towers.Length; i++)
        {
            if (towers[i].TryGetComponent(out NetworkIdentity networkIdentity))
            {
                if (networkIdentity.netId == towerNetId)
                {
                    IUpgradeOption upgradeOption;
                    if (upgradeIndex == -1)
                    {
                        upgradeOption = towers[i].GetComponent<IUpgradeable>().CurrentUpgrade;
                    }
                    else
                    {
                        upgradeOption = towers[i].GetComponent<IUpgradeable>().UpgradeOptions[upgradeIndex];
                    }

                    towers[i].OnUpgradeStarted(upgradeOption, out bool upgradeStarted);
                }
            }
        }
    }

    [Server]
    public void UpdateCurrency(uint playersNetId, int value, Vector3 position)
    {
        _currency += value;

        var conn = FindObjectsOfType<NetworkIdentity>().FirstOrDefault(x => x.netId == playersNetId).connectionToClient;
        TargetDisplayCurrencyChange(conn, value, position);

        RpcUpdateCurrency(playersNetId, _currency);
    }

    [TargetRpc]
    public void TargetDisplayCurrencyChange(NetworkConnection target, int value, Vector3 position)
    {
        GameController.Instance.DisplayCurrencyChange(value, position);
    }

    [ClientRpc]
    public void RpcUpdateCurrency(uint playersNetId, int currency)
    {
        EventManager.ExecuteEvent<IPlayerEvents>((x, y) => x.OnCurrencyUpdated(playersNetId, currency));
    }

    [Server]
    public void UpdateLives(uint playersNetId, int value, Vector3 position)
    {
        _lives += value;

        var conn = FindObjectsOfType<NetworkIdentity>().FirstOrDefault(x => x.netId == playersNetId).connectionToClient;
        TargetDisplayLivesChange(conn, value, position);

        RpcUpdateLives(playersNetId, _lives);

        if (_lives <= 0)
        {
            GameController.Instance.GameOver();
        }
    }

    [ClientRpc]
    public void RpcUpdateLives(uint playersNetId, int lives)
    {
        EventManager.ExecuteEvent<IPlayerEvents>((x, y) => x.OnLivesUpdated(playersNetId, lives));
    }

    [TargetRpc]
    public void TargetDisplayLivesChange(NetworkConnection target, int value, Vector3 position)
    {
        GameController.Instance.DisplayLivesChange(value, position);
    }

    [Server]
    public void AddEnemiesToCreepCount(int count)
    {
        _creeps += count;
        RpcUpdateCreeps(MyInfo.netId, _creeps);
        
    }

    [Server]
    public void RemoveEnemyFromCreepCount()
    {
        _creeps--;
        RpcUpdateCreeps(MyInfo.netId, _creeps);
    }

    [ClientRpc]
    private void RpcUpdateCreeps(uint playersNetId, int creeps)
    {
        EventManager.ExecuteEvent<IPlayerEvents>((x, y) => x.OnCreepsUpdated(playersNetId, creeps));
    }
}
