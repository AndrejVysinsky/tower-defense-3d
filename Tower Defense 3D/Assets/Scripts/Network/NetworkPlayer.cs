using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    private List<NetworkConnection> _networkConnections = new List<NetworkConnection>();
    private List<GameObject> _spawnPrefabs;

    public Boundaries PlayerBoundaries { get; set; }

    private Boundaries _mapBoundaries;
    private CameraController _cameraController;
    private GridController _gridController;

    private void Awake()
    {
        _cameraController = FindObjectOfType<CameraController>();
        _gridController = FindObjectOfType<GridController>();
        _spawnPrefabs = FindObjectOfType<NetworkManager>().spawnPrefabs;
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

    public void PlayerConnected(NetworkConnection networkConnection)
    {
        _networkConnections.Add(networkConnection);
    }

    public void PlayerDisconnected(NetworkConnection networkConnection)
    {
        _networkConnections.Remove(networkConnection);
    }

    public void Spawn(int spawnableIndex, Vector3 pos, Quaternion rotation)
    {
        CmdSpawnTower(spawnableIndex, pos, rotation, GetComponent<NetworkIdentity>().netId);
    }

    [Command]
    private void CmdSpawnTower(int spawnableIndex, Vector3 pos, Quaternion rotation, uint playersNetId)
    {
        var playerGameObject = FindObjectsOfType<NetworkIdentity>().FirstOrDefault(x => x.netId == playersNetId).gameObject;

        var objectToSpawn = Instantiate(_spawnPrefabs[spawnableIndex], pos, rotation);
        NetworkServer.Spawn(objectToSpawn, playerGameObject);

        RpcUpgradeTower(objectToSpawn.GetComponent<NetworkIdentity>().netId);
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
}
