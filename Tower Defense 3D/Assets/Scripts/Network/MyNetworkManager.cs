using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    private List<NetworkConnection> _networkConnections = new List<NetworkConnection>();

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        NetworkPlayer connectedPlayer = conn.identity.GetComponent<NetworkPlayer>();

        foreach (NetworkConnection networkConnection in _networkConnections)
        {
            var networkPlayer = networkConnection.identity.GetComponent<NetworkPlayer>();
            networkPlayer.PlayerConnected(conn);
            connectedPlayer.PlayerConnected(networkConnection);
        }

        _networkConnections.Add(conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);

        _networkConnections.Remove(conn);

        foreach (NetworkConnection networkConnection in _networkConnections)
        {
            var networkPlayer = networkConnection.identity.GetComponent<NetworkPlayer>();
            networkPlayer.PlayerDisconnected(conn);
        }
    }
}
