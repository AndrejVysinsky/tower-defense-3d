using Mirror;
using UnityEngine;

public class LeaveGameHandler : NetworkBehaviour
{
    public void LeaveGame()
    {
        var players = FindObjectsOfType<NetworkPlayer>();
        var networkManager = FindObjectOfType<MyNetworkManager>();
        var sceneLoader = FindObjectOfType<SceneLoader>();

        networkManager.StopHost();
        sceneLoader.ChangeScene(0);
    }
}
