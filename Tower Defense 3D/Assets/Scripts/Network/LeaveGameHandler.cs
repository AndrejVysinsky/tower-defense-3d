using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaveGameHandler : MonoBehaviour
{
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Menu Scene")
        {
            FindObjectOfType<MyLobbyManager>()?.DisableCallbacks();

            if (NetworkManager.singleton != null)
            {
                Destroy(NetworkManager.singleton.gameObject);
                NetworkManager.Shutdown();
            }

            var networkObject = GameObject.Find("Network Manager");
            if (networkObject != null)
            {
                Destroy(networkObject);
            }
        }
    }

    public void LeaveGame()
    {
        var sceneLoader = FindObjectOfType<SceneLoader>();
        if (NetworkManager.singleton == null)
        {
            sceneLoader.ChangeScene(0);
            return;
        }

        LobbyConfig.Instance.SetLobbyStatus(LobbyConfig.LobbyStatus.Left);

        FindObjectOfType<MyLobbyManager>()?.DisableCallbacks();
        Destroy(NetworkManager.singleton.gameObject);
        NetworkManager.Shutdown();

        sceneLoader.ChangeScene(0);
    }
}
