using Assets.Scripts.Network;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LobbyPlayerList : MonoBehaviour, IServerEvents
{
    [SerializeField] GameObject lobbyPlayerStatusPrefab;
    [SerializeField] GameObject lobbyPlayerStatusContainer;
    
    private NetworkPlayer _localPlayer;
    private List<LobbyPlayerStatus> _playerList;

    private void Awake()
    {
        _playerList = new List<LobbyPlayerStatus>();
    }

    public void OnPlayerInitialized(NetworkPlayer networkPlayer)
    {
        if (_localPlayer == null)
        {
            _localPlayer = NetworkClient.localPlayer.gameObject.GetComponent<NetworkPlayer>();
        }

        AddPlayerDisplay(networkPlayer);
    }

    public void OnPlayerDisconnected(uint playerId)
    {
        RemovePlayerDisplay(playerId);
    }

    private void OnEnable()
    {
        EventManager.AddListener(gameObject);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(gameObject);
    }

    private void AddPlayerDisplay(NetworkPlayer networkPlayer)
    {
        //if player does not have status, create it
        uint playerId = networkPlayer.GetComponent<NetworkIdentity>().netId;
        if (_playerList.Any(x => x.Id == playerId) == true)
        {
            Debug.Log(playerId + " already in status list");
            return;
        }

        var playerStatusObject = Instantiate(lobbyPlayerStatusPrefab, lobbyPlayerStatusContainer.transform);

        var playerStatus = playerStatusObject.GetComponent<LobbyPlayerStatus>();
        playerStatus.SetPlayerStatus(playerId, networkPlayer.MyInfo.name, networkPlayer.MyInfo.color, networkPlayer.PlayerImage);

        _playerList.Add(playerStatus);
    }

    private void RemovePlayerDisplay(uint playerId)
    {
        var playerStatus = _playerList.FirstOrDefault(x => x.Id == playerId);
        _playerList.Remove(playerStatus);
        Destroy(playerStatus.gameObject);
    }
}