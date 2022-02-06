using Mirror;
using UnityEngine;

public class LeaveGameHandler : NetworkBehaviour
{
    public void LeaveGame()
    {
        var players = FindObjectsOfType<NetworkPlayer>();
        var networkManager = FindObjectOfType<MyNetworkManager>();
        var sceneLoader = FindObjectOfType<SceneLoader>();

        foreach (var player in players)
        {
            if (player.isLocalPlayer == false)
                continue;

            if (player.isServer)
            {
                networkManager.StopHost();
            }
            else
            {
                networkManager.StopClient();
            }
        }
        
        sceneLoader.ChangeScene(0);
    }
}
