using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerStatus : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Image playerImage;

    public uint Id { get; private set; }

    private void Awake()
    {
        SetPlayerStatus(0, "Name not set", Color.gray, null);
    }

    public void SetPlayerStatus(uint id, string name, Color color, Sprite sprite)
    {
        Id = id;

        if (NetworkClient.localPlayer.netId == id)
        {
            name += " (you)";
        }

        //idText.text = id.ToString();
        nameText.color = color;
        nameText.text = name;

        playerImage.sprite = sprite;
    }
}