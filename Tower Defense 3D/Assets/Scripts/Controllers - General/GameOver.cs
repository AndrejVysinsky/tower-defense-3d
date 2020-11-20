using TMPro;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;

    public void Initialize(int score)
    {
        scoreText.text = score.ToString();
    }
}