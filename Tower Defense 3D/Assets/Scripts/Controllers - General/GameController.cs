using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [SerializeField] GameInfoDisplay gameInfoDisplay;
    //[SerializeField] GameOver gameOver;
    [SerializeField] int startingCurrency;
    [SerializeField] int startingLives;

    public int Currency { get; private set; }
    public int Lives { get; private set; }

    private int _wave;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Currency = startingCurrency;
        Lives = startingLives;

        gameInfoDisplay.UpdateCurrencyText(startingCurrency);
        gameInfoDisplay.UpdateWaveText(_wave);
        gameInfoDisplay.UpdateLivesText(startingLives);
    }

    public void ModifyCurrencyBy(int value)
    {
        UpdateCurrency(value);
    }

    public void ModifyCurrencyBy(int value, Vector3 position)
    {
        gameInfoDisplay.DisplayCurrencyChangeText(value, position);
        UpdateCurrency(value);
    }

    private void UpdateCurrency(int value)
    {
        Currency += value;
        gameInfoDisplay.UpdateCurrencyText(Currency);

        //EventManager.Instance.ExecuteEvent<ICurrencyChanged>((x, y) => x.OnCurrencyChanged(Currency));
    }

    public void ModifyLivesBy(int value)
    {
        UpdateLives(value);
    }

    public void ModifyLivesBy(int value, Vector3 position)
    {
        gameInfoDisplay.DisplayLivesChangeText(value, position);
        UpdateLives(value);
    }

    private void UpdateLives(int value)
    {
        Lives += value;
        gameInfoDisplay.UpdateLivesText(Lives);

        //if (Lives <= 0)
        //{
        //    if (gameOver.gameObject.activeSelf)
        //        return;

        //    Time.timeScale = 0f;

        //    gameOver.gameObject.SetActive(true);

        //    gameOver.InitializeScore(_wave, 100, false);
        //    // gameOver.InitializeScore(250, 3000, true);
        //}
    }

    public void WaveSpawned(int waveNumber)
    {
        _wave = waveNumber;

        gameInfoDisplay.UpdateWaveText(waveNumber);
    }
}