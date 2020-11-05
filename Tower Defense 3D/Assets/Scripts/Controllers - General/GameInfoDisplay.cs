using TMPro;
using UnityEngine;

public class GameInfoDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currencyText;
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] TextMeshProUGUI livesText;

    public void UpdateCurrencyText(int total)
    {
        currencyText.text = GetParsedCurrency(total);
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

    public void DisplayCurrencyChangeText(int change, Vector3 position)
    {
        //TODO: spawn text at position
    }

    public void UpdateLivesText(int total)
    {
        //TODO: spawn text at position
        livesText.text = total.ToString();
    }

    public void DisplayLivesChangeText(int change, Vector3 position)
    {
        //TODO: spawn text at position
    }

    public void UpdateWaveText(int waveNumber)
    {
        waveText.text = waveNumber.ToString();
    }
}
