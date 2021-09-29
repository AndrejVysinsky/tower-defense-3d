using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    private List<NetworkConnection> _networkConnections = new List<NetworkConnection>();

    #region NetworkManager methods
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

    #endregion

    public List<NetworkPlayer> GetNetworkPlayers()
    {
        List<NetworkPlayer> networkPlayers = new List<NetworkPlayer>();

        for (int i = 0; i < _networkConnections.Count; i++)
        {
            if (_networkConnections[i] != null && _networkConnections[i].identity != null)
            {
                networkPlayers.Add(_networkConnections[i].identity.GetComponent<NetworkPlayer>());
            }
        }
        return networkPlayers;
    }
}
