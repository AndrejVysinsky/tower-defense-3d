using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    private List<NetworkConnection> _networkConnections = new List<NetworkConnection>();

    public void PlayerConnected(NetworkConnection networkConnection)
    {
        _networkConnections.Add(networkConnection);
    }

    public void PlayerDisconnected(NetworkConnection networkConnection)
    {
        _networkConnections.Remove(networkConnection);
    }
}
