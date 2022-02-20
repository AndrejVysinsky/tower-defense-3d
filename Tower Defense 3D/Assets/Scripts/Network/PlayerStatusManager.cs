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

        private List<PlayerStatus> _playerStatusList;

        private NetworkPlayer _localPlayer;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _playerStatusList = new List<PlayerStatus>();                        
        }

        public void OnPlayerInitialized(NetworkPlayer networkPlayer)
        {
            if (_localPlayer == null)
            {
                _localPlayer = NetworkClient.localPlayer.gameObject.GetComponent<NetworkPlayer>();
            }

            AddPlayerDisplay(networkPlayer);
        }

        public void OnPlayerDisconnected(uint playerId)
        {
            RemovePlayerDisplay(playerId);
        }

        private void OnEnable()
        {
            EventManager.AddListener(gameObject);
        }

        private void OnDisable()
        {
            EventManager.RemoveListener(gameObject);
        }

        public void OnColorUpdated(uint playersNetId, Color color)
        {
            for (int i = 0; i < _playerStatusList.Count; i++)
            {
                if (_playerStatusList[i].Id == playersNetId)
                {
                    _playerStatusList[i].SetColor(color);
                }
            }
        }

        public void OnCurrencyUpdated(uint playersNetId, int currentValue)
        {
            for (int i = 0; i < _playerStatusList.Count; i++)
            {
                if (_playerStatusList[i].Id == playersNetId)
                {
                    _playerStatusList[i].SetCurrency(currentValue);
                }
            }
        }

        public void OnLivesUpdated(uint playersNetId, int currentValue)
        {
            for (int i = 0; i < _playerStatusList.Count; i++)
            {
                if (_playerStatusList[i].Id == playersNetId)
                {
                    _playerStatusList[i].SetLives(currentValue);
                }
            }
        }

        public void OnCreepsUpdated(uint playersNetId, int currentValue)
        {
            for (int i = 0; i < _playerStatusList.Count; i++)
            {
                if (_playerStatusList[i].Id == playersNetId)
                {
                    _playerStatusList[i].SetCreeps(currentValue);
                }
            }
        }

        private void AddPlayerDisplay(NetworkPlayer networkPlayer)
        {
            //if player does not have status, create it
            uint playerId = networkPlayer.GetComponent<NetworkIdentity>().netId;
            if (_playerStatusList.Any(x => x.Id == playerId) == true)
            {
                Debug.Log(playerId + " already in status list");
                return;
            }

            var playerStatusObject = Instantiate(playerStatusPrefab, playerStatusContainer.transform);

            var playerStatus = playerStatusObject.GetComponent<PlayerStatus>();
            playerStatus.SetPlayerStatus(playerId, networkPlayer.MyInfo.steamId, networkPlayer.MyInfo.name, networkPlayer.MyInfo.color, networkPlayer.Lives, networkPlayer.Currency);

            _playerStatusList.Add(playerStatus);


            var playerStatusHeight = playerStatusPrefab.GetComponent<RectTransform>().sizeDelta.y;
            _rectTransform.SetHeight(heightWithoutPlayers + _playerStatusList.Count * playerStatusHeight);

            //if player has status but is disconnected, delete it / grey out
            //or use ondisconnect in network manager
        }

        private void RemovePlayerDisplay(uint playerId)
        {
            var playerStatus = _playerStatusList.FirstOrDefault(x => x.Id == playerId);
            _playerStatusList.Remove(playerStatus);
            Destroy(playerStatus.gameObject);
        }
    }
}
