using TMPro;
using UnityEngine;

public class FloatingTextScript : MonoBehaviour
{
    [SerializeField] float distance;
    [SerializeField] float speed;
    [SerializeField] TextMeshProUGUI textField;

    private Transform _myTransform;
    private float _currentDistance;

    private float _fadeOutTreshold;
    private float _fadeOutDistance;

    private bool _isFading;

    private void Awake()
    {
        _myTransform = transform;

        _fadeOutTreshold = distance / 3 * 2;
        _fadeOutDistance = distance - _fadeOutTreshold;
    }

    public void Initialize(string value, Vector3 position)
    {
        transform.position = position;
        textField.text = value;

        _currentDistance = 0;
        _isFading = false;
    }

    private void Update()
    {
        ShiftPosition();

        FadeOutText();
        
        if (_currentDistance >= distance)
        {
            Destroy(gameObject);
        }
    }

    private void ShiftPosition()
    {
        var position = _myTransform.position;

        position.y += speed * Time.deltaTime;
        _currentDistance += speed * Time.deltaTime;

        _myTransform.position = position;
    }

    private void FadeOutText()
    {
        if (_currentDistance >= _fadeOutTreshold)
            _isFading = true;

        if (_isFading == false)
            return;

        var color = textField.color;

        var alpha =  1 - ((_currentDistance - _fadeOutTreshold) / _fadeOutDistance);

        if (alpha < 0)
            alpha = 0;

        color.a = alpha;

        textField.color = color;
    }
}