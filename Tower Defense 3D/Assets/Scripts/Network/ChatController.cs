using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatController : NetworkBehaviour
{
    [SerializeField] GameObject messageContainer;
    [SerializeField] GameObject chatMessagePrefab;

    [SerializeField] TMP_InputField chatInput;

    private NetworkPlayer _localPlayer;

    public struct ChatMessage : NetworkMessage
    {
        public string message;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<ChatMessage>(OnChatMessageSend);
    }

    private void OnChatMessageSend(NetworkConnection conn, ChatMessage chatMessage)
    {
        if (string.IsNullOrEmpty(chatMessage.message))
        {
            return;
        }

        RpcDisplayMessage(conn.identity.netId, chatMessage.message);
    }

    public void SendChatMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
            return;

        if (_localPlayer == null)
        {
            var players = FindObjectsOfType<NetworkPlayer>();
            foreach (var player in players)
            {
                if (player.isLocalPlayer)
                {
                    _localPlayer = player;
                    break;
                }
            }
        }

        ChatMessage chatMessage = new ChatMessage { message = message };
        NetworkClient.Send(chatMessage);
    }

    [ClientRpc]
    private void RpcDisplayMessage(uint playerId, string message)
    {
        var player = GetPlayerById(playerId);

        string hexColorValue = ColorUtility.ToHtmlStringRGB(player.MyInfo.color);

        var messageObject = Instantiate(chatMessagePrefab, messageContainer.transform);
        messageObject.GetComponent<TMP_Text>().text = $"<color=#{hexColorValue}>{player.MyInfo.name}</color>: {message}";

        LayoutRebuilder.ForceRebuildLayoutImmediate(messageContainer.GetComponent<RectTransform>());
    }

    public void OnTextChange()
    {
        if (chatInput.text.EndsWith("\n"))
        {
            SendChatMessage(chatInput.text.Remove(chatInput.text.Length - 1));
            chatInput.text = "";
        }
    }

    private NetworkPlayer GetPlayerById(uint playerId)
    {
        var players = FindObjectsOfType<NetworkPlayer>();
        foreach (var player in players)
        {
            if (player.MyInfo.netId == playerId)
            {
                return player;
            }
        }
        return null;
    }
}