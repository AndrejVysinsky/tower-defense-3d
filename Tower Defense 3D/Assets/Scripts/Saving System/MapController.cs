using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : NetworkBehaviour
{
    [SerializeField] MapSaveManager saveManager;

    [Server]
    public void LoadMap(string mapHash, bool isCustomMap)
    {
        var myNetworkManager = (MyNetworkManager)NetworkManager.singleton;

        RpcLoadMap(myNetworkManager.GetPlayerIds(), mapHash, isCustomMap);
    }

    [ClientRpc]
    private void RpcLoadMap(List<uint> playerIds, string mapHash, bool isCustomMap)
    {
        var mapName = FileManager.DefaultMaps[0];
        if (isCustomMap)
        {
            var fileNames = FileManager.GetFiles(FileManager.MapPath);
            foreach (var fileName in fileNames)
            {
                FileManager.LoadFile(FileManager.MapPath, fileName, out MapSaveData mapSaveData);

                if (mapSaveData.GetMapHash() == mapHash)
                {
                    mapName = fileName;
                    break;
                }
            }
        }
        else
        {
            var resourceFileNames = FileManager.DefaultMaps;
            foreach (var resourceFile in resourceFileNames)
            {
                FileManager.LoadResourceFile(FileManager.ResourceMapPath, resourceFile, out MapSaveData mapSaveData);

                if (mapSaveData.GetMapHash() == mapHash)
                {
                    mapName = resourceFile;
                    break;
                }
            }
        }

        saveManager.LoadMapData(false, playerIds, mapName, isCustomMap);
    }
}
