using Mirror;
using Steamworks;
using UnityEngine;

public class MyLobbyManager : MonoBehaviour
{
    private MyNetworkManager _myNetworkManager;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private const string hostAddressKey = "HostAddress";

    public static CSteamID LobbyId { get; private set; } = CSteamID.Nil;

    void Start()
    {
        _myNetworkManager = GetComponent<MyNetworkManager>();

        if (!SteamManager.Initialized)
        {
            InitializeMirrorLobby();
        }
        else
        {
            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

            if (LobbyConfig.Instance.GetLobbyType() == LobbyConfig.LobbyType.Host)
            {
                InitializeSteamLobby();
            }
            else if (LobbyConfig.Instance.GetLobbyType() == LobbyConfig.LobbyType.Client)
            {
                var lobbyIdString = LobbyConfig.Instance.GetLobbyId();

                if (string.IsNullOrEmpty(lobbyIdString))
                {
                    return;
                }

                if (ulong.TryParse(lobbyIdString, out ulong lobbyId))
                {
                    JoinLobby(lobbyId);
                }
            }
        }
    }

    public void DisableCallbacks()
    {
        lobbyCreated = null;
        gameLobbyJoinRequested = null;
        lobbyEntered = null;

        SteamMatchmaking.LeaveLobby(LobbyId);
    }

    private void InitializeSteamLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, _myNetworkManager.maxConnections);
    }

    private void InitializeMirrorLobby()
    {
        if (LobbyConfig.Instance.GetLobbyType() == LobbyConfig.LobbyType.Host)
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
        if (callback.m_eResult != EResult.k_EResultOK || _myNetworkManager == null)
        {
            return;
        }

        LobbyId = new CSteamID(callback.m_ulSteamIDLobby);
        FindObjectOfType<LobbyIdInput>().SetLobbyId(LobbyId.m_SteamID.ToString());

        _myNetworkManager.StartHost();
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey, SteamUser.GetSteamID().ToString());
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        JoinLobby(callback.m_ulSteamIDLobby);
    }

    private void JoinLobby(ulong lobbyId)
    {
        //server is active on this client -> is host
        if (NetworkServer.active)
        {
            return;
        }

        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(lobbyId), hostAddressKey);

        _myNetworkManager.networkAddress = hostAddress;
        _myNetworkManager.StartClient();
    }
}
