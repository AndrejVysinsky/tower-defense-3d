using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class MapController : NetworkBehaviour
{
    [SerializeField] MapSaveManager saveManager;

    [Server]
    public void LoadMap()
    {
        //TODO:
        //1. get number of players connected
        //2. pass it to RpcLoadMap so every player has own map

        //get list of connected players and pass it to map loader
        var myNetworkManager = (MyNetworkManager)NetworkManager.singleton;

        RpcLoadMap(myNetworkManager.GetNetworkPlayers());
    }

    [ClientRpc]
    private void RpcLoadMap(List<NetworkPlayer> networkPlayers)
    {
        saveManager.LoadMapData(false, networkPlayers);
    }
}
