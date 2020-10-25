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
    private float _currentHealth;

    private Camera _mainCamera;

    public float Health => _currentHealth;

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

        _mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward, _mainCamera.transform.rotation * Vector3.up);
    }

    public void Initialize(float health)
    {
        _currentHealth = health;
        _maxHealth = health;
    }

    public void SubtractHealth(float amount)
    {
        _currentHealth -= amount;

        if (_currentHealth < 0)
            _currentHealth = 0;
        
        VisualiseDamage();
    }

    private void VisualiseDamage()
    {
        float value = (float)_currentHealth / _maxHealth;

        slider.value = value;
        healthIndicator.color = _gradient.Evaluate(value);
    }
}
