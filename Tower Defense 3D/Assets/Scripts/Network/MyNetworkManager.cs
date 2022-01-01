using Assets.Scripts.Network;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    private List<NetworkConnection> _networkConnections = new List<NetworkConnection>();

    private List<Color> _colors = new List<Color>()
    {
        //https://www.imgonline.com.ua/eng/color-palette.php

        new Color32(255,99,71,255), //tomato
        new Color32(255,140,0,255), //dark orange
        new Color32(255,215,0,255), //gold
        new Color32(173,255,47,255), //green yellow
        new Color32(64,224,208,255), //turquoise
        new Color32(0,191,255,255), //deep sky blue
        new Color32(255,105,80,255), //hot pink
        new Color32(255,160,122,255), //light salmon
        new Color32(255,228,181,255), //moccasin
        new Color32(240,230,140,255), //khaki
        new Color32(143,188,143,255), //dark sea green
        new Color32(144,238,144,255), //light green
        new Color32(135,206,235,255), //sky blue
        new Color32(230,230,250,255), //lavender
    };

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

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        int random = Random.Range(0, _colors.Count);

        BasePlayerInfo addedPlayerInfo = new BasePlayerInfo();
        addedPlayerInfo.netId = conn.identity.netId;
        addedPlayerInfo.name = "Player" + conn.identity.netId;
        addedPlayerInfo.color = _colors[random];

        _playerInfoList.Add(addedPlayerInfo);

        _colors.RemoveAt(random);

        NetworkPlayer addedPlayer = conn.identity.GetComponent<NetworkPlayer>();

        //add his own info
        addedPlayer.PlayerConnected(addedPlayerInfo.netId, addedPlayerInfo.name, addedPlayerInfo.color);

        foreach (NetworkConnection networkConnection in _networkConnections)
        {
            var connectedPlayer = networkConnection.identity.GetComponent<NetworkPlayer>();
            
            //tell connected players about added player
            connectedPlayer.PlayerConnected(addedPlayerInfo.netId, addedPlayerInfo.name, addedPlayerInfo.color);

            //tell added player about connected players
            var connectedPlayerInfo = _playerInfoList.FirstOrDefault(x => x.netId == networkConnection.identity.netId);
            addedPlayer.PlayerConnected(connectedPlayerInfo.netId, connectedPlayerInfo.name, connectedPlayerInfo.color);
        }

        _networkConnections.Add(conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);

        _networkConnections.Remove(conn);

        //foreach (NetworkConnection networkConnection in _networkConnections)
        //{
        //    var networkPlayer = networkConnection.identity.GetComponent<NetworkPlayer>();
        //    networkPlayer.PlayerDisconnected(conn.identity.netId);
        //}
    }
    #endregion

    public List<uint> GetPlayerIds()
    {
        return _networkConnections.Select(x => x.identity.netId).ToList();
    }
}
