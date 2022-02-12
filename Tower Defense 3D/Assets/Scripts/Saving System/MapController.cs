using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class MapController : NetworkBehaviour
{
    [SerializeField] MapSaveManager saveManager;

    [Server]
    public void LoadMap(string mapName, bool isCustomMap)
    {
        //TODO:
        //1. get number of players connected
        //2. pass it to RpcLoadMap so every player has own map

        //get list of connected players and pass it to map loader
        var myNetworkManager = (MyNetworkManager)NetworkManager.singleton;

        Debug.Log("Load Map called");

        RpcLoadMap(myNetworkManager.GetPlayerIds(), mapName, isCustomMap);
    }

    [ClientRpc]
    private void RpcLoadMap(List<uint> playerIds, string mapName, bool isCustomMap)
    {
        Debug.Log("RPC Load Map called");

        saveManager.LoadMapData(false, playerIds, mapName, isCustomMap);
    }
}
