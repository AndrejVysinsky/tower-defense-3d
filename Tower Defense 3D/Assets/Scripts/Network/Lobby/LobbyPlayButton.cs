using Assets.Scripts.Network;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayButton : MonoBehaviour, IServerEvents
{
    [SerializeField] GameObject button;

    private void Awake()
    {
        button.SetActive(false);
    }

    private void OnEnable()
    {
        EventManager.AddListener(gameObject);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(gameObject);
    }

    public void ChangeToGameScene()
    {
        var mapExplorer = FindObjectOfType<MapExplorer>();

        var selectedMap = mapExplorer.SelectedMapCard.MapName;
        var isCustomMap = mapExplorer.SelectedMapCard.IsCustomMap;

        LobbyConfig.Instance.SetSelectedMap(selectedMap, isCustomMap);
        LobbyConfig.Instance.SetLobbyStatus(LobbyConfig.LobbyStatus.InGame);
        LobbyConfig.Instance.SetLobbyPlayerCount(NetworkServer.connections.Count);
        NetworkManager.singleton.ServerChangeScene("Game Scene");
    }

    public void OnPlayerInitialized(NetworkPlayer networkPlayer)
    {
        if (networkPlayer.isLocalPlayer && networkPlayer.isServer)
        {
            button.SetActive(true);
        }
    }

    public void OnPlayerDisconnected(uint playerId)
    {
    }
}
