using UnityEngine;

public class MyLobbyManager : MonoBehaviour
{
    private MyNetworkManager _myNetworkManager;

    void Start()
    {
        _myNetworkManager = GetComponent<MyNetworkManager>();

        InitializeLobby();
    }

    private void InitializeLobby()
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
}
