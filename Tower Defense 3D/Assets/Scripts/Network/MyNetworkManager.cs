using Assets.Scripts.Network;
using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager
{
    private List<NetworkConnection> _networkConnections = new List<NetworkConnection>();

    private List<Color> _colors = new List<Color>()
    {
        new Color32(255,50,50,255), //red
        new Color32(49,115,255,255), //blue
        new Color32(255,139,49,255), //orange
        new Color32(55,186,86,255), //green
        new Color32(250,255,51,255), //yellow
        new Color32(45,255,153,255), //cyan to green?
        new Color32(44,227,255,255), //light blue
        new Color32(110,62,255,255), //purple
    };

    private int _colorIndex = 0;

    private List<BasePlayerInfo> _playerInfoList = new List<BasePlayerInfo>();

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

    public override void OnServerChangeScene(string newSceneName)
    {
        base.OnServerChangeScene(newSceneName);
        _networkConnections.Clear();
        _colorIndex = 0;
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        BasePlayerInfo addedPlayerInfo = new BasePlayerInfo();

        if (SteamManager.Initialized)
        {
            CSteamID steamId = SteamMatchmaking.GetLobbyMemberByIndex(MyLobbyManager.LobbyId, numPlayers - 1);
            addedPlayerInfo.steamId = steamId.m_SteamID;
        }

        addedPlayerInfo.netId = conn.identity.netId;
        addedPlayerInfo.name = "Player" + (_networkConnections.Count + 1);
        addedPlayerInfo.color = _colors[_colorIndex++];

        _playerInfoList.Add(addedPlayerInfo);

        NetworkPlayer addedPlayer = conn.identity.GetComponent<NetworkPlayer>();

        //add his own info
        addedPlayer.PlayerConnected(addedPlayerInfo.netId, addedPlayerInfo.steamId, addedPlayerInfo.name, addedPlayerInfo.color);

        foreach (NetworkConnection networkConnection in _networkConnections)
        {
            var connectedPlayer = networkConnection.identity.GetComponent<NetworkPlayer>();
            
            //tell connected players about added player
            connectedPlayer.PlayerConnected(addedPlayerInfo.netId, addedPlayerInfo.steamId, addedPlayerInfo.name, addedPlayerInfo.color);

            //tell added player about connected players
            var connectedPlayerInfo = _playerInfoList.FirstOrDefault(x => x.netId == networkConnection.identity.netId);
            addedPlayer.PlayerConnected(connectedPlayerInfo.netId, addedPlayerInfo.steamId, connectedPlayerInfo.name, connectedPlayerInfo.color);
        }

        var waitForPlayers = FindObjectOfType<WaitForDownload>();
        if (waitForPlayers != null)
        {
            waitForPlayers.PlayerJoined(conn);
        }

        _networkConnections.Add(conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        _networkConnections.Remove(conn);
        foreach (var connection in _networkConnections)
        {
            connection.identity.GetComponent<NetworkPlayer>().PlayerDisconnected(conn.identity.netId);
        }
        base.OnServerDisconnect(conn);
    }

    public override void ServerChangeScene(string newSceneName)
    {
        foreach (NetworkConnection connection in _networkConnections)
        {
            connection.identity.GetComponent<NetworkPlayer>().ChangeLobbyStatus(LobbyConfig.LobbyStatus.SceneChange);
        }

        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        foreach (NetworkConnection connection in _networkConnections)
        {
            connection.identity.GetComponent<NetworkPlayer>().ChangeLobbyStatus(LobbyConfig.LobbyStatus.InGame);
        }

        base.OnServerSceneChanged(sceneName);
    }

    #endregion

    public List<uint> GetPlayerIds()
    {
        return _networkConnections.Select(x => x.identity.netId).ToList();
    }
}
