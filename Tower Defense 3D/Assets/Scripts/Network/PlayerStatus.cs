using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public class PlayerStatus : LobbyPlayerStatus
    {
        [SerializeField] TextMeshProUGUI livesText;
        [SerializeField] TextMeshProUGUI currencyText;

        protected override void Awake()
        {
            SetPlayerStatus(0, 0, "Name not set", Color.gray, 0, 0);
        }

        public void SetPlayerStatus(uint id, ulong steamId, string name, Color color, int lives, int currency)
        {
            SetPlayerStatus(id, steamId, name, color);
            
            if (livesText != null)
            {
                livesText.text = lives.ToString();
            }

            if (currencyText != null)
            {
                currencyText.text = currency.ToString();
            }
        }

        public void SetLives(int lives)
        {
            if (livesText == null)
                return;

            livesText.text = lives.ToString();
        }

        public void SetCurrency(int currency)
        {
            if (currencyText == null)
                return;

            currencyText.text = GetParsedCurrency(currency);
        }

        private string GetParsedCurrency(float value)
        {
            char thousandsChar = default;

            if (value / 1000000 >= 1)
            {
                value /= 1000000;
                thousandsChar = 'm';
            }
            else if (value / 1000 >= 1)
            {
                value /= 1000;
                thousandsChar = 'k';
            }

            return value.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) + thousandsChar;
        }
    }
}
