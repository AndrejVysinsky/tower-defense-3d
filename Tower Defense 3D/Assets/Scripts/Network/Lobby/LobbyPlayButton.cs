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
        LobbyConfig.Instance.SetLobbyStatus(LobbyConfig.LobbyStatus.InGame);
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
