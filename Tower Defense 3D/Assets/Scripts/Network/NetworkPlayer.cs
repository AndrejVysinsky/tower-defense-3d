using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    private List<NetworkPlayer> _networkPlayers = new List<NetworkPlayer>();

    private int counter = 0;

    public void PlayerConnected(NetworkPlayer networkPlayer)
    {
        _networkPlayers.Add(networkPlayer);
    }

    public void PlayerDisconnected(NetworkPlayer networkPlayer)
    {
        _networkPlayers.Remove(networkPlayer);
    }

    public void Counter()
    {
        counter++;
        Debug.Log(NetworkClient.localPlayer.connectionToServer.connectionId + ": " + counter);
    }
}
