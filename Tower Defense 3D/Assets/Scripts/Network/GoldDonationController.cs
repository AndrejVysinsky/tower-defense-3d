using Assets.Scripts.Network;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GoldDonationController : NetworkBehaviour, IPlayerEvents
{
    [SerializeField] GameObject goldDonationButton;
    [SerializeField] GameObject goldDonationMenu;
    [SerializeField] TMP_Dropdown playerDropDown;
    [SerializeField] TMP_InputField goldInput;
    [SerializeField] TMP_Text maxGoldText;
    
    private bool _isMultiplayer;

    public struct GoldDonationMessage : NetworkMessage
    {
        public uint fromPlayer;
        public uint toPlayer;
        public int amount;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<GoldDonationMessage>(OnGoldDonationMessageSend);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        NetworkServer.UnregisterHandler<GoldDonationMessage>();
    }

    private void OnEnable()
    {
        EventManager.AddListener(gameObject);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(gameObject);
    }

    public void OnMenuOpened()
    {
        _isMultiplayer = FindObjectsOfType<NetworkPlayer>().Length > 1;
        goldDonationButton.SetActive(_isMultiplayer);
    }

    public void ShowDonationMenu()
    {
        GetComponent<GameController>().PauseGame(); //only to hide pause menu

        goldDonationMenu.SetActive(true);
        InitializeMenu();
    }

    public void HideDonationMenu()
    {
        goldDonationMenu.SetActive(false);
    }

    private void InitializeMenu()
    {
        var players = FindObjectsOfType<NetworkPlayer>();

        var playerNames = new List<string>();
        foreach (var player in players)
        {
            if (player.isLocalPlayer)
                continue;

            playerNames.Add($"<color=#{ColorUtility.ToHtmlStringRGB(player.MyInfo.color)}>{player.MyInfo.name}</color>");
        }

        playerDropDown.ClearOptions();
        playerDropDown.AddOptions(playerNames);
    }

    public void SendGold()
    {
        if (string.IsNullOrEmpty(goldInput.text))
            return;

        if (int.TryParse(goldInput.text, out int goldAmount))
        {
            int index = playerDropDown.value;
            var option = playerDropDown.options[index];
            var players = FindObjectsOfType<NetworkPlayer>();

            NetworkPlayer fromPlayer = null;
            NetworkPlayer toPlayer = null;
            foreach (var player in players)
            {
                if (player.isLocalPlayer)
                {
                    fromPlayer = player;
                }

                if (option.text.Contains(player.MyInfo.name))
                {
                    toPlayer = player;
                }               
            }

            if (fromPlayer == null || toPlayer == null)
                return;

            GoldDonationMessage goldDonationMessage = new GoldDonationMessage
            {
                fromPlayer = fromPlayer.netId,
                toPlayer = toPlayer.netId,
                amount = goldAmount
            };
            NetworkClient.Send(goldDonationMessage);
        }
    }

    private void OnGoldDonationMessageSend(NetworkConnection conn, GoldDonationMessage goldDonationMessage)
    {
        var players = FindObjectsOfType<NetworkPlayer>();

        NetworkPlayer fromPlayer = null;
        NetworkPlayer toPlayer = null;

        foreach (var player in players)
        {
            if (player.MyInfo.netId == goldDonationMessage.fromPlayer)
                fromPlayer = player;

            if (player.MyInfo.netId == goldDonationMessage.toPlayer)
                toPlayer = player;
        }

        if (fromPlayer == null || toPlayer == null)
            return;

        fromPlayer.UpdateCurrency(fromPlayer.MyInfo.netId, -goldDonationMessage.amount, new Vector3(-100, -100, -100)); //somewhere out of screen
        toPlayer.UpdateCurrency(toPlayer.MyInfo.netId, goldDonationMessage.amount, new Vector3(-100, -100, -100)); //somewhere out of screen
    }

    public void OnColorUpdated(uint playersNetId, Color color)
    {
    }

    public void OnCurrencyUpdated(uint playersNetId, int currentValue)
    {
        var player = FindObjectsOfType<NetworkPlayer>().FirstOrDefault(x => x.netId == playersNetId);
        if (player.isLocalPlayer)
        {
            maxGoldText.text = "Max: " + currentValue;
        }
    }

    public void OnLivesUpdated(uint plyersNetId, int currentValue)
    {
    }

    public void OnCreepsUpdated(uint plyersNetId, int currentValue)
    {
    }
}
