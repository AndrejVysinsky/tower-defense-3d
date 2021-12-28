using Assets.Scripts.Network;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    private List<NetworkConnection> _networkConnections = new List<NetworkConnection>();

    #region NetworkManager methods
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
    }

    /*
     * NetworkManager.singleton.StopClient();
     * NetworkManager.singleton.StopHost();
     * 
     */

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        NetworkPlayer connectedPlayer = conn.identity.GetComponent<NetworkPlayer>();

        //add his own connection
        connectedPlayer.PlayerConnected(conn.identity.netId);

        foreach (NetworkConnection networkConnection in _networkConnections)
        {
            var networkPlayer = networkConnection.identity.GetComponent<NetworkPlayer>();
            networkPlayer.PlayerConnected(conn.identity.netId);
            connectedPlayer.PlayerConnected(networkConnection.identity.netId);
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
            networkPlayer.PlayerDisconnected(conn.identity.netId);
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
                var networkPlayer = _networkConnections[i].identity.GetComponent<NetworkPlayer>();
                networkPlayers.Add(networkPlayer);
            }
        }
        return networkPlayers;
    }

    public List<uint> GetPlayerIds()
    {
        return _networkConnections.Select(x => x.identity.netId).ToList();
    }

    public List<NetworkPlayer> GetNetworkPlayers(SyncList<uint> connectedPlayers)
    {
        List<NetworkPlayer> networkPlayers = new List<NetworkPlayer>();

        for (int i = 0; i < connectedPlayers.Count; i++)
        {
            var networkIdentity = FindObjectsOfType<NetworkIdentity>().FirstOrDefault(x => x.netId == connectedPlayers[i]).gameObject;
            var networkPlayer = networkIdentity.GetComponent<NetworkPlayer>();
            networkPlayers.Add(networkPlayer);
        }
        return networkPlayers;
    }
}
