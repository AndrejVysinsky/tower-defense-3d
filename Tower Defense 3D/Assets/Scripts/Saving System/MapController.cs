using Mirror;
using UnityEngine;

public class MapController : NetworkBehaviour
{
    [SerializeField] MapSaveManager saveManager;

    [Server]
    public void LoadMap()
    {
        RpcLoadMap();
    }

    [ClientRpc]
    private void RpcLoadMap()
    {
        saveManager.LoadMapData(false);
    }
}
