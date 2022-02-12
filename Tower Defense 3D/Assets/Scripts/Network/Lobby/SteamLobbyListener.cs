using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SteamLobbyListener : MonoBehaviour
{
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;

    void Start()
    {
        if (!SteamManager.Initialized)
        {
            return;
        }

        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("lobby join requested steam listener");

        LobbyConfig.Instance.SetLobbyId(callback.m_steamIDLobby.m_SteamID.ToString());
        SceneManager.LoadScene("Lobby Scene");
    }
}
