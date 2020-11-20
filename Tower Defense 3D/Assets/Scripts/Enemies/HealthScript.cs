using UnityEngine;
using UnityEngine.UI;

public class HealthScript : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Image healthIndicator;

    [SerializeField] Color32 fullHealthColor;
    [SerializeField] Color32 lowHealthColor;

    private Gradient _gradient;
    private float _maxHealth;

    public float Health { get; private set; }

    private void Awake()
    {
        _gradient = new Gradient()
        {
            colorKeys = new GradientColorKey[]
            {
                new GradientColorKey(lowHealthColor, 0f),
                new GradientColorKey(fullHealthColor, 1f)
            }
        };
    }

    public void Initialize(float health)
    {
        Health = health;
        _maxHealth = health;
    }

    public void SubtractHealth(float amount)
    {
        Health -= amount;

        if (Health < 0)
            Health = 0;
        
        VisualiseDamage();
    }

    private void VisualiseDamage()
    {
        float value = Health / _maxHealth;

        slider.value = value;
        healthIndicator.color = _gradient.Evaluate(value);
    }
}
