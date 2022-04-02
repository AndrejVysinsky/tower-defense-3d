using UnityEngine;
using UnityEngine.UI;

public class LobbyGameModeSelector : MonoBehaviour
{
    [SerializeField] Image versusImage;
    [SerializeField] Image coopImage;

    [SerializeField] Color activeColor;
    [SerializeField] Color inactiveColor;

    private void Start()
    {
        SelectVersusMode();
    }

    public void SelectVersusMode()
    {
        versusImage.color = activeColor;
        coopImage.color = inactiveColor;

        LobbyConfig.Instance.SetLobbyMode(LobbyConfig.LobbyMode.Versus);
    }

    public void SelectCoopMode()
    {
        versusImage.color = inactiveColor;
        coopImage.color = activeColor;

        LobbyConfig.Instance.SetLobbyMode(LobbyConfig.LobbyMode.Coop);
    }
}