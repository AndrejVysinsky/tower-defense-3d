﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyConfig : MonoBehaviour
{
    private readonly string lobbyModeKey = "LobbyMode";
    private readonly string lobbyStatusKey = "LobbyStatus";
    private readonly string lobbyTypeKey = "LobbyType";
    private readonly string lobbyIdKey = "LobbyId";
    private readonly string selectedMapKey = "SelectedMap";
    private readonly string selectedMapIsCustomKey = "SelectedMapIsCustom";
    private readonly string playerCountKey = "PlayerCount";

    public enum LobbyMode
    {
        Versus,
        Coop
    }

    public enum LobbyStatus
    {
        InLobby,
        SceneChange,
        InGame,
        Left
    }
    
    public enum LobbyType
    {
        Host,
        Client
    }

    public static LobbyConfig Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Menu Scene")
        {
            ResetLobbyData();
        }
    }

    private void ResetLobbyData()
    {
        PlayerPrefs.DeleteKey(lobbyModeKey);
        PlayerPrefs.DeleteKey(lobbyStatusKey);
        PlayerPrefs.DeleteKey(lobbyTypeKey);
        PlayerPrefs.DeleteKey(lobbyIdKey);
        PlayerPrefs.DeleteKey(selectedMapKey);
        PlayerPrefs.DeleteKey(selectedMapIsCustomKey);
    }

    public void SetLobbyMode(LobbyMode lobbyMode)
    {
        PlayerPrefs.SetInt(lobbyModeKey, (int)lobbyMode);
    }

    public void SetLobbyStatus(LobbyStatus lobbyStatus)
    {
        PlayerPrefs.SetInt(lobbyStatusKey, (int)lobbyStatus);
    }

    public void SetLobbyType(int lobbyType)
    {
        PlayerPrefs.SetInt(lobbyTypeKey, lobbyType);
    }

    public void SetLobbyId(string lobbyId)
    {
        PlayerPrefs.SetString(lobbyIdKey, lobbyId);
    }

    public void SetSelectedMap(string selectedMap, bool isCustomMap)
    {
        PlayerPrefs.SetString(selectedMapKey, selectedMap);
        PlayerPrefs.SetInt(selectedMapIsCustomKey, isCustomMap == true ? 1 : 0);
    }

    public void SetLobbyPlayerCount(int playerCount)
    {
        PlayerPrefs.SetInt(playerCountKey, playerCount);
    }

    public LobbyMode GetLobbyMode()
    {
        if (PlayerPrefs.HasKey(lobbyModeKey))
        {
            return (LobbyMode)PlayerPrefs.GetInt(lobbyModeKey);
        }
        return LobbyMode.Versus;
    }

    public LobbyType GetLobbyType()
    {
        if (PlayerPrefs.HasKey(lobbyTypeKey))
        {
            return (LobbyType)PlayerPrefs.GetInt(lobbyTypeKey);
        }
        return LobbyType.Host;
    }

    public LobbyStatus GetLobbyStatus()
    {
        if (PlayerPrefs.HasKey(lobbyStatusKey))
        {
            return (LobbyStatus)PlayerPrefs.GetInt(lobbyStatusKey);
        }
        return LobbyStatus.InLobby;
    }

    public string GetLobbyId()
    {
        if (PlayerPrefs.HasKey(lobbyIdKey))
        {
            return PlayerPrefs.GetString(lobbyIdKey);
        }
        return null;
    }

    public (bool isCustomMap, string mapName) GetSelectedMap()
    {
        if (PlayerPrefs.HasKey(selectedMapKey))
        {
            return (PlayerPrefs.GetInt(selectedMapIsCustomKey) == 1, PlayerPrefs.GetString(selectedMapKey));
        }
        return (false, FileManager.DefaultMaps[0]);
    }

    public int GetPlayerCount()
    {
        if (PlayerPrefs.HasKey(playerCountKey))
        {
            return PlayerPrefs.GetInt(playerCountKey);
        }
        return 1;
    }
}
