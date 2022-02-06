using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyConfig : MonoBehaviour
{
    private readonly string lobbyStatusKey = "LobbyStatus";
    private readonly string lobbyTypeKey = "LobbyType";
    private readonly string joinTypeKey = "JoinType";

    public enum LobbyStatus
    {
        Waiting,
        InGame
    }
    
    public enum LobbyType
    {
        Mirror,
        Steam
    }

    public enum JoinType
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

    public void SetLobbyStatus(LobbyStatus lobbyStatus)
    {
        PlayerPrefs.SetInt(lobbyStatusKey, (int)lobbyStatus);
    }

    public void SetLobbyType(int lobbyType)
    {
        PlayerPrefs.SetInt(lobbyTypeKey, lobbyType);
    }

    public void SetJoinType(int joinType)
    {
        PlayerPrefs.SetInt(joinTypeKey, joinType);
    }

    public LobbyStatus GetLobbyStatus()
    {
        if (PlayerPrefs.HasKey(lobbyStatusKey))
        {
            return (LobbyStatus)PlayerPrefs.GetInt(lobbyStatusKey);
        }
        return LobbyStatus.Waiting;
    }

    public LobbyType GetLobbyType()
    {
        if (PlayerPrefs.HasKey(lobbyTypeKey))
        {
            return (LobbyType)PlayerPrefs.GetInt(lobbyTypeKey);
        }
        return LobbyType.Steam;
    }

    public JoinType GetJoinType()
    {
        if (PlayerPrefs.HasKey(joinTypeKey))
        {
            return (JoinType)PlayerPrefs.GetInt(joinTypeKey);
        }
        return JoinType.Host;
    }
}
