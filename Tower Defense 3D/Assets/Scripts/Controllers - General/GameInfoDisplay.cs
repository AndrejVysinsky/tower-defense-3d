using TMPro;
using UnityEngine;

public class GameInfoDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI waveText;

    [SerializeField] GameObject floatingTextPrefab;
    
    public void DisplayCurrencyChangeText(int change, Vector3 position)
    {
        var floatingText = Instantiate(floatingTextPrefab);
        floatingText.GetComponent<FloatingTextScript>().Initialize(change.ToString(), position);
    }

    public void DisplayLivesChangeText(int change, Vector3 position)
    {
        var floatingText = Instantiate(floatingTextPrefab);
        floatingText.GetComponent<FloatingTextScript>().Initialize(change.ToString(), position);
    }

    public void UpdateWaveText(int waveNumber)
    {
        waveText.text = waveNumber.ToString();
    }
}
