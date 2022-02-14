using Mirror;
using Steamworks;
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
    [SerializeField] RawImage playerImage;

    public uint Id { get; private set; }
    public ulong SteamId { get; private set; }

    protected Callback<AvatarImageLoaded_t> avatarImageLoaded;

    protected virtual void Awake()
    {
        SetPlayerStatus(0, 0, "Name not set", Color.gray);
    }

    public void SetPlayerStatus(uint id, ulong steamId, string name, Color color)
    {
        avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);

        Id = id;
        SteamId = steamId;

        if (NetworkClient.localPlayer.netId == id)
        {
            name += " (you)";
        }

        if (steamId != 0)
        {
            CSteamID cSteamId = new CSteamID(steamId);
            name = SteamFriends.GetFriendPersonaName(cSteamId);

            int imageId = SteamFriends.GetLargeFriendAvatar(cSteamId);

            if (imageId != -1)
            {
                playerImage.texture = GetSteamImageAsTexture(imageId);
            }
        }

        nameText.color = color;
        nameText.text = name;
    }

    private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID != SteamId)
        {
            return;
        }

        playerImage.texture = GetSteamImageAsTexture(callback.m_iImage);
    }

    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);

        if (isValid)
        {
            //4 bytes per pixel - RGBA
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }

        return texture;
    }
}