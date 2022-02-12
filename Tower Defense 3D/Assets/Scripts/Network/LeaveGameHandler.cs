using Mirror;
using System.Collections;
using UnityEngine;

public class LeaveGameHandler : MonoBehaviour
{
    public void LeaveGame()
    {
        if (NetworkManager.singleton == null)
        {
            var sceneLoader = FindObjectOfType<SceneLoader>();
            sceneLoader.ChangeScene(0);
            return;
        }

        //if (NetworkServer.active)
        //{
        //    NetworkManager.singleton.StopHost();
        //}
        //else
        //{
        //    NetworkManager.singleton.StopClient();
        //}

        FindObjectOfType<MyLobbyManager>()?.DisableCallbacks();
        Destroy(NetworkManager.singleton.gameObject);
        NetworkManager.Shutdown();
    }
}
