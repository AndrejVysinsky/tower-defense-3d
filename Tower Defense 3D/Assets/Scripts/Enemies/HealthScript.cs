using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class HealthScript : NetworkBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Image healthIndicator;

    [SerializeField] Color32 fullHealthColor;
    [SerializeField] Color32 lowHealthColor;

    private Gradient _gradient;

    public float Health => _health;
    public float MaxHealth { get; private set; }

    [SyncVar(hook = nameof(UpdateHealthHook))]
    private float _health;

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
        _health = health;
        MaxHealth = health;
    }

    [Server]
    public void SubtractHealth(float amount)
    {
        _health -= amount;

        if (_health < 0)
            _health = 0;
    }

    public void UpdateHealthHook(float oldHealth, float newHealth)
    {
        _health = newHealth;

        VisualiseDamage();
    }

    private void VisualiseDamage()
    {
        float value = _health / MaxHealth;

        slider.value = value;
        healthIndicator.color = _gradient.Evaluate(value);
    }
}
