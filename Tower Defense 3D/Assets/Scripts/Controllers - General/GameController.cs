using Mirror;
using UnityEngine;

public class GameController : NetworkBehaviour
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

        gameInfoDisplay.UpdateWaveText(_wave);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    public void DisplayCurrencyChange(int value, Vector3 position)
    {
        gameInfoDisplay.DisplayCurrencyChangeText(value, position);
    }

    public void DisplayLivesChange(int value, Vector3 position)
    {
        gameInfoDisplay.DisplayLivesChangeText(value, position);
    }

    public void WaveSpawned(int waveNumber)
    {
        _wave = waveNumber;

        gameInfoDisplay.UpdateWaveText(waveNumber);
    }

    public void GameOver()
    {
        gameOver.gameObject.SetActive(true);

        gameOver.Initialize(_score);

        Time.timeScale = 0f;
    }

    public void PauseGame()
    {
        if (gameOver.gameObject.activeSelf)
            return;

        if (_isPaused)
        {
            //Time.timeScale = 1;
            pauseMenu.gameObject.SetActive(false);
        }
        else
        {
            //Time.timeScale = 0;
            pauseMenu.gameObject.SetActive(true);
            pauseMenu.Initialize(_score);

            GetComponent<GoldDonationController>().OnMenuOpened();
            GetComponent<GoldDonationController>().HideDonationMenu();
        }

        _isPaused = !_isPaused;
    }
}