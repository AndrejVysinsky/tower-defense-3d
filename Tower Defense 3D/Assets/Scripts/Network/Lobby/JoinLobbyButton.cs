using TMPro;
using UnityEngine;

public class JoinLobbyButton : MonoBehaviour
{
    [SerializeField] SceneLoader sceneLoader;
    [SerializeField] TMP_InputField steamIdText;

    public void JoinSteamLobby()
    {
        if (SteamManager.Initialized && string.IsNullOrWhiteSpace(steamIdText.text))
        {
            return;
        }

        LobbyConfig.Instance.SetLobbyType(1);
        LobbyConfig.Instance.SetLobbyId(steamIdText.text);
        sceneLoader.ChangeScene(1);
    }
}
