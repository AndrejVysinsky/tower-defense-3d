using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [SerializeField] GameInfoDisplay gameInfoDisplay;
    [SerializeField] MenuScoreDisplay gameOver;
    [SerializeField] MenuScoreDisplay pauseMenu;
    [SerializeField] int startingCurrency;
    [SerializeField] int startingLives;

    public int Currency { get; private set; }
    public int Lives { get; private set; }

    private int _wave;
    private int _score;

    private bool _isPaused = false;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
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

        if (value > 0)
            _score += value;

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

        if (Lives <= 0)
        {
            gameOver.gameObject.SetActive(true);

            gameOver.Initialize(_score);

            Time.timeScale = 0f;
        }
    }

    public void WaveSpawned(int waveNumber)
    {
        _wave = waveNumber;

        gameInfoDisplay.UpdateWaveText(waveNumber);
    }

    public void PauseGame()
    {
        if (gameOver.gameObject.activeSelf)
            return;

        if (_isPaused)
        {
            Time.timeScale = 1;
            pauseMenu.gameObject.SetActive(false);
        }
        else
        {
            Time.timeScale = 0;
            pauseMenu.gameObject.SetActive(true);
            pauseMenu.Initialize(_score);
        }

        _isPaused = !_isPaused;
    }
}