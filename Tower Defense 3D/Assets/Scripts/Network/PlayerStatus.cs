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
            SetPlayerStatus(0, "Name not set", 0, 0);
        }

        public void SetPlayerStatus(uint id, string name, int lives, int currency)
        {
            Id = id;

            //idText.text = id.ToString();
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
            currencyText.text = currency.ToString();
        }
    }
}
