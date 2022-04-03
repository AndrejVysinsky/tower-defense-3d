using Assets.Scripts.Network;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InterestRateController : NetworkBehaviour, IPlayerEvents
{
    [SerializeField] TMP_Text expectedInterestValue;
    [SerializeField] Image timerFill;

    [SyncVar] float _timer;

    private float _interestRate = 0.02f;
    private float _interval = 15;

    private void OnEnable()
    {
        EventManager.AddListener(gameObject);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(gameObject);
    }

    private void Start()
    {
        UpdateExpectedValue();
    }

    private void Update()
    {
        if (isServer)
        {
            _timer += Time.deltaTime;
            if (_timer > _interval)
            {
                _timer = 0;
                AddInterest();
            }
        }

        timerFill.fillAmount = _timer / _interval;
    }

    [Server]
    private void AddInterest()
    {
        var players = FindObjectsOfType<NetworkPlayer>();

        foreach (var player in players)
        {
            var interestValue = (int)(player.Currency * _interestRate);

            player.UpdateCurrency(player.MyInfo.netId, interestValue, new Vector3(-100, -100, -100));
        }
    }

    private void UpdateExpectedValue()
    {
        var players = FindObjectsOfType<NetworkPlayer>();
        foreach (var player in players)
        {
            if (player.isLocalPlayer)
            {
                var interestValue = (int)(player.Currency * _interestRate);
                expectedInterestValue.text = $"+{interestValue}";
            }
        }
    }

    public void OnColorUpdated(uint playersNetId, Color color)
    {
    }

    public void OnCurrencyUpdated(uint playersNetId, int currentValue)
    {
        UpdateExpectedValue();
    }

    public void OnLivesUpdated(uint plyersNetId, int currentValue)
    {
    }

    public void OnCreepsUpdated(uint plyersNetId, int currentValue)
    {
    }
}
