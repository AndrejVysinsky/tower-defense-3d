using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public class PlayerStatusManager : MonoBehaviour, IPlayerEvents, IServerEvents
    {
        [SerializeField] int heightWithoutPlayers = 60;
        [SerializeField] GameObject playerStatusPrefab;
        [SerializeField] GameObject playerStatusContainer;

        private MyNetworkManager _networkManager;
        private List<PlayerStatus> _playerStatusList;

        private NetworkPlayer _localPlayer;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _playerStatusList = new List<PlayerStatus>();                        
        }

        public void OnPlayerInitialized()
        {
            _localPlayer = NetworkClient.localPlayer.gameObject.GetComponent<NetworkPlayer>();
            _networkManager = (MyNetworkManager)NetworkManager.singleton;

            var networkPlayers = _networkManager.GetNetworkPlayers(_localPlayer.GetPlayerConnections());
            UpdatePlayerDisplay(networkPlayers);
        }

        private void OnEnable()
        {
            EventManager.AddListener(gameObject);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener(gameObject);
        }

        public void OnCurrencyUpdated(uint playersNetId, int currentValue)
        {
            var networkPlayers = _networkManager.GetNetworkPlayers(_localPlayer.GetPlayerConnections());
            UpdatePlayerDisplay(networkPlayers);

            for (int i = 0; i < networkPlayers.Count; i++)
            {
                if (_playerStatusList[i].Id == playersNetId)
                {
                    _playerStatusList[i].SetCurrency(currentValue);
                }
            }
        }

        public void OnLivesUpdated(uint playersNetId, int currentValue)
        {
            var networkPlayers = _networkManager.GetNetworkPlayers(_localPlayer.GetPlayerConnections());
            UpdatePlayerDisplay(networkPlayers);

            for (int i = 0; i < networkPlayers.Count; i++)
            {
                if (_playerStatusList[i].Id == playersNetId)
                {
                    _playerStatusList[i].SetLives(currentValue);
                }
            }
        }

        private void UpdatePlayerDisplay(List<NetworkPlayer> networkPlayers)
        {
            //if player does not have status, create it
            for (int i = 0; i < networkPlayers.Count; i++)
            {
                uint playerId = networkPlayers[i].GetComponent<NetworkIdentity>().netId;
                if (_playerStatusList.Any(x => x.Id == playerId) == true)
                {
                    continue;
                }
                
                var playerStatusObject = Instantiate(playerStatusPrefab, playerStatusContainer.transform);

                var playerStatus = playerStatusObject.GetComponent<PlayerStatus>();
                playerStatus.SetPlayerStatus(playerId, "Player " + playerId, networkPlayers[i].Lives, networkPlayers[i].Currency);

                _playerStatusList.Add(playerStatus);
            }

            var playerStatusHeight = playerStatusPrefab.GetComponent<RectTransform>().sizeDelta.y + 10;
            _rectTransform.SetHeight(heightWithoutPlayers + networkPlayers.Count * playerStatusHeight);

            //if player has status but is disconnected, delete it / grey out
            //or use ondisconnect in network manager
        }
    }
}
