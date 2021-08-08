using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    private List<NetworkConnection> _networkConnections = new List<NetworkConnection>();

    public Boundaries PlayerBoundaries { get; set; }

    private Boundaries _mapBoundaries;
    private CameraController _cameraController;

    private void Awake()
    {
        _cameraController = FindObjectOfType<CameraController>();
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
}
