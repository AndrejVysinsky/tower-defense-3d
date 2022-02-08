using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SteamLobbyListener : MonoBehaviour
{
    protected Callback<LobbyEnter_t> lobbyEntered;

    void Start()
    {
        if (!SteamManager.Initialized)
        {
            return;
        }
        
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        //server is active on this client -> is host
        if (NetworkServer.active)
        {
            return;
        }

        LobbyConfig.Instance.SetLobbyId(callback.m_ulSteamIDLobby.ToString());
        SceneManager.LoadScene("Lobby Scene");
    }
}
