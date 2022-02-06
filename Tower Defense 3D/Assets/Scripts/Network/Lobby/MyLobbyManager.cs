using Mirror;
using Steamworks;
using System;
using UnityEngine;

public class MyLobbyManager : MonoBehaviour
{
    private MyNetworkManager _myNetworkManager;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private const string hostAddressKey = "HostAddress";

    void Start()
    {
        _myNetworkManager = GetComponent<MyNetworkManager>();

        //if (LobbyConfig.Instance.GetLobbyType() == LobbyConfig.LobbyType.Mirror)
        //{
        //    InitializeMirrorLobby();
        //    return;
        //}

        if (!SteamManager.Initialized)
            return;

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        if (LobbyConfig.Instance.GetJoinType() == LobbyConfig.JoinType.Host)
        {
            InitializeSteamLobby();
        }
    }

    private void InitializeSteamLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, _myNetworkManager.maxConnections);
    }

    private void InitializeMirrorLobby()
    {
        if (LobbyConfig.Instance.GetJoinType() == LobbyConfig.JoinType.Host)
        {
            _myNetworkManager.StartHost();
        }
        else
        {
            _myNetworkManager.StartClient();
        }
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            return;
        }

        _myNetworkManager.StartHost();
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey, SteamUser.GetSteamID().ToString());
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        //server is active on this client - host
        if (NetworkServer.active)
        {
            return;
        }

        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey);

        _myNetworkManager.networkAddress = hostAddress;
        _myNetworkManager.StartClient();
    }
}
