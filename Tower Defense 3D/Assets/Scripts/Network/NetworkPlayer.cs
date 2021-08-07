using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    private List<NetworkConnection> _networkConnections = new List<NetworkConnection>();

    public PlayersBoundaries Boundaries { get; set; }
    public class PlayersBoundaries
    {
        public int MinX { get; set; }
        public int MaxX { get; set; }
        public int MinZ { get; set; }
        public int MaxZ { get; set; }
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
