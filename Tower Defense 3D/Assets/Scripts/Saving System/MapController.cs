using Mirror;
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

        //connection can be null, should add check later
        int numberOfPlayers = NetworkServer.connections.Count;

        RpcLoadMap(numberOfPlayers);
    }

    [ClientRpc]
    private void RpcLoadMap(int numberOfPlayers)
    {
        saveManager.LoadMapData(false, numberOfPlayers);
    }
}
