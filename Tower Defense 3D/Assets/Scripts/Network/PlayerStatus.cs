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
    public class PlayerStatus : MonoBehaviour
    {
        //[SerializeField] TextMeshProUGUI idText;
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] TextMeshProUGUI livesText;
        [SerializeField] TextMeshProUGUI currencyText;

        public uint Id { get; private set; }

        private void Awake()
        {
            SetPlayerStatus(0, "Name not set", Color.gray, 0, 0);
        }

        public void SetPlayerStatus(uint id, string name, Color color, int lives, int currency)
        {
            Id = id;

            if (NetworkClient.localPlayer.netId == id)
            {
                name += " (you)";
            }

            //idText.text = id.ToString();
            nameText.color = color;
            nameText.text = name;
            
            livesText.text = lives.ToString();
            currencyText.text = currency.ToString();
        }

        public void SetLives(int lives)
        {
            livesText.text = lives.ToString();
        }

        public void SetCurrency(int currency)
        {
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
