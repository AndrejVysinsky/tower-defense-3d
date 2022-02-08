using TMPro;
using UnityEngine;

public class LobbyIdInput : MonoBehaviour
{
    [SerializeField] TMP_InputField idText;

    public string GetLobbyId()
    {
        return idText.text;
    }

    public void SetLobbyId(string lobbyId)
    {
        idText.text = lobbyId;
    }
}
