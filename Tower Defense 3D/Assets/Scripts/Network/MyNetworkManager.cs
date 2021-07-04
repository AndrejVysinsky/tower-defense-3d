using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    private List<NetworkPlayer> _networkPlayers = new List<NetworkPlayer>();

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        Debug.Log($"Player was added! There are {numPlayers} players connected.");

        NetworkPlayer connectedPlayer = conn.identity.gameObject.GetComponent<NetworkPlayer>();

        foreach (NetworkPlayer networkPlayer in _networkPlayers)
        {
            networkPlayer.PlayerConnected(connectedPlayer);
            connectedPlayer.PlayerConnected(networkPlayer);
        }

        _networkPlayers.Add(connectedPlayer);
    }
}
