using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    private List<NetworkConnection> _networkConnections = new List<NetworkConnection>();
    private List<GameObject> _spawnPrefabs;

    public Boundaries PlayerBoundaries { get; set; }

    private Boundaries _mapBoundaries;
    private CameraController _cameraController;
    private GridController _gridController;
    private GameObject _objectToSpawn;

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
        CmdSpawn(spawnableIndex, pos, rotation);
    }

    [Command]
    private void CmdSpawn(int spawnableIndex, Vector3 pos, Quaternion rotation)
    {
        var objectToSpawn = Instantiate(_spawnPrefabs[spawnableIndex], pos, rotation);

        if (objectToSpawn.TryGetComponent(out IUpgradeable upgradeable))
        {
            upgradeable.OnUpgradeStarted(upgradeable.CurrentUpgrade, out bool upgradeStarted);

            if (upgradeStarted == false)
            {
                Destroy(objectToSpawn);
                return;
            }
        }

        NetworkServer.Spawn(objectToSpawn, NetworkClient.localPlayer.gameObject);
    }
}
