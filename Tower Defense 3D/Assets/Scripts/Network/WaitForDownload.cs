using Assets.Scripts.Network;
using Mirror;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;

public class WaitForDownload : NetworkBehaviour
{
    [SerializeField] TMP_Text playerCountText;

    [Header("Player status")]
    [SerializeField] Color notReadyStatusColor;
    [SerializeField] Color readyStatusColor;
    [SerializeField] TMP_Text statusText;

    [SyncVar(hook = nameof(OnCurrentPlayerCountUpdated))]
    private int _playersReady;

    [SyncVar]
    private int _totalPlayerCount;

    private int _currentPlayerCount;
    private byte[] _mapByteArray;
    private string _mapName;
    private string _mapHash;
    private bool _isCustomMap;

    private float waitingTime = 0;
    private float maxWaitingTime = 20;
    private bool maxWaitExceeded = false;

    public struct HasMapMessage : NetworkMessage
    {
        public bool hasMap;
    }

    private void Awake()
    {
        playerCountText.text = $"Waiting for players... ({_playersReady}/{_totalPlayerCount})";
    }

    [ServerCallback]
    private void Update()
    {
        if (maxWaitExceeded)
            return;

        if (waitingTime > maxWaitingTime)
        {
            HandleNotConnectedPlayers();
            maxWaitExceeded = true;
        }

        waitingTime += Time.deltaTime;
    }

    private void HandleNotConnectedPlayers()
    {

    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        _totalPlayerCount = LobbyConfig.Instance.GetPlayerCount();

        var map = LobbyConfig.Instance.GetSelectedMap();

        MapSaveData mapSaveData;

        if (map.isCustomMap)
        {
            FileManager.LoadFile(FileManager.MapPath, map.mapName, out mapSaveData);
        }
        else
        {
            FileManager.LoadResourceFile(FileManager.ResourceMapPath, map.mapName, out mapSaveData);
        }

        _mapByteArray = ByteSerializer.ObjectToByteArray(mapSaveData);
        _mapHash = mapSaveData.GetMapHash();
        _mapName = map.mapName;
        _isCustomMap = map.isCustomMap;

        NetworkServer.RegisterHandler<HasMapMessage>(OnHasMapMessage);
    }

    private void OnHasMapMessage(NetworkConnection conn, HasMapMessage hasMapMessage)
    {
        if (hasMapMessage.hasMap)
        {
            _playersReady++;
            OnPlayersReadyUpdated();
        }
        else
        {
            TargetDownloadMap(conn, _mapByteArray, _mapName);
        }
    }

    [Server]
    private void OnPlayersReadyUpdated()
    {
        if (_playersReady == _totalPlayerCount)
        {
            StartCoroutine(WaitForNetworkUpdate());
        }
    }

    [Server]
    private IEnumerator WaitForNetworkUpdate()
    {
        yield return new WaitForSecondsRealtime(1);

        FindObjectOfType<MapController>().LoadMap(_mapHash, _isCustomMap);
        RpcHideWaitMessage();
    }

    [ClientRpc]
    public void RpcHideWaitMessage()
    {
        gameObject.SetActive(false);
    }

    [Server]
    public void PlayerJoined(NetworkConnection connection)
    {
        _currentPlayerCount++;

        if (_isCustomMap)
        {
            TargetCheckMapAlreadyDownloaded(connection, _mapHash);
        }
        else
        {
            _playersReady++;
            TargetChangeStatusToReady(connection);
            OnPlayersReadyUpdated();
        }        
    }

    [TargetRpc]
    private void TargetChangeStatusToReady(NetworkConnection target)
    {
        ChangeStatusText(true);
    }

    [TargetRpc]
    public void TargetCheckMapAlreadyDownloaded(NetworkConnection target, string mapHash)
    {
        var fileNames = FileManager.GetFiles(FileManager.MapPath);
        MapSaveData mapSaveData;
        bool hasMap = false;
        foreach (var fileName in fileNames)
        {
            FileManager.LoadFile(FileManager.MapPath, fileName, out mapSaveData);

            if (mapSaveData.GetMapHash() == mapHash)
            {
                hasMap = true;
                break;
            }
        }

        ChangeStatusText(hasMap);

        HasMapMessage hasMapMessage = new HasMapMessage { hasMap = hasMap };
        NetworkClient.Send(hasMapMessage);
    }

    [TargetRpc]
    public void TargetDownloadMap(NetworkConnection target, byte[] mapByteArray, string mapName)
    {
        var mapSaveData = ByteSerializer.ByteArrayToObject<MapSaveData>(mapByteArray);

        //save locally
        var localFiles = FileManager.GetFiles(FileManager.MapPath);
        var i = 1;
        var uniqueMapName = mapName;
        while (localFiles.Contains(uniqueMapName))
        {
            uniqueMapName = $"{mapName} ({i++})";
        }
        FileManager.SaveFile(FileManager.MapPath, uniqueMapName, mapSaveData);

        ChangeStatusText(true);
        
        HasMapMessage hasMapMessage = new HasMapMessage { hasMap = true };
        NetworkClient.Send(hasMapMessage);
    }

    public void OnCurrentPlayerCountUpdated(int oldValue, int newValue)
    {
        playerCountText.text = $"Waiting for players... ({newValue}/{_totalPlayerCount})";
    }

    private void ChangeStatusText(bool isReady)
    {
        if (isReady)
        {
            statusText.text = "Ready";
            statusText.color = readyStatusColor;
        }
        else
        {
            statusText.text = "Not ready";
            statusText.color = notReadyStatusColor;
        }
    }
}
