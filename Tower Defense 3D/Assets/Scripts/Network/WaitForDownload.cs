using Assets.Scripts.Network;
using Mirror;
using System;
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
        ChangeStatusText(false);
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

        Debug.Log("Server started");

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
        _mapName = map.mapName;
        _isCustomMap = map.isCustomMap;

        NetworkServer.RegisterHandler<HasMapMessage>(OnHasMapMessage);
    }

    private void OnHasMapMessage(NetworkConnection conn, HasMapMessage hasMapMessage)
    {
        if (hasMapMessage.hasMap)
        {
            //player is ready to play
            _playersReady++;
            Debug.Log("player ready");
        }
        else
        {
            //send map to player
            TargetDownloadMap(conn, _mapByteArray, _mapName);
        }

        if (_playersReady == _totalPlayerCount)
        {
            FindObjectOfType<MapController>().LoadMap(_mapName, _isCustomMap);
            RpcHideWaitMessage();
        }
    }

    [ClientRpc]
    public void RpcHideWaitMessage()
    {
        gameObject.SetActive(false);
    }

    [Server]
    public void PlayerJoined(NetworkConnection connection)
    {
        Debug.Log("Player joined");

        _currentPlayerCount++;
        //gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(connection);

        //if (_isCustomMap)
        //{

        //}
        TargetCheckMapAlreadyDownloaded(connection, "superHash");
    }

    [TargetRpc]
    public void TargetCheckMapAlreadyDownloaded(NetworkConnection target, string mapHash)
    {
        var fileNames = FileManager.GetFiles(FileManager.MapPath);
        //MapSaveData mapSaveData;
        bool hasMap = false;
        //foreach (var fileName in fileNames)
        //{
        //    FileManager.LoadFile(FileManager.MapPath, fileName, out mapSaveData);

        //    //check for mapHash value
        //    if (true)
        //    {
        //        hasMap = true; 
        //        break;
        //    }
        //}

        Debug.Log("map check on client, hasMap = " + hasMap);
        //CmdNotifyAboutMap(hasMap);

        HasMapMessage hasMapMessage = new HasMapMessage { hasMap = hasMap };
        NetworkClient.Send(hasMapMessage);
    }

    [TargetRpc]
    public void TargetDownloadMap(NetworkConnection target, byte[] mapByteArray, string mapName)
    {
        var mapSaveData = ByteSerializer.ByteArrayToObject<MapSaveData>(mapByteArray);
        
        //save locally
        //mapName = $"{mapName} - {DateTime.Now}";
        //FileManager.SaveFile(FileManager.MapPath, mapName, mapSaveData);

        Debug.Log("Map download called");
        ChangeStatusText(true);
        //CmdNotifyAboutMap(true);
        HasMapMessage hasMapMessage = new HasMapMessage { hasMap = true };
        NetworkClient.Send(hasMapMessage);
    }

    public void OnCurrentPlayerCountUpdated(int oldValue, int newValue)
    {
        Debug.Log("Hook called");

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
