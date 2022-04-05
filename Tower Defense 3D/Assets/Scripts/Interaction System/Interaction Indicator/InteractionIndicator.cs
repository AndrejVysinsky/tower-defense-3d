using UnityEngine;
using UnityEngine.AI;

public class InteractionIndicator : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] int vertexCount = 40;
    [SerializeField] float lineWidth = 0.05f;

    private Transform _interactingObject;
    private Vector3 _lastPosition;
    private float _radius;
    private float _heighOffset;

    private readonly float _distanceAboveGround = 0.1f;
    private readonly float _agentExtraRadius = 0.15f;

    private void OnEnable()
    {
        ShowIndicator();
    }

    private void Update()
    {
        if (_lastPosition == _interactingObject.position)
            return;

        _lastPosition = _interactingObject.position;

        SetPosition(_lastPosition);
    }

    public void ShowIndicator()
    {
        _interactingObject = InteractionSystem.Instance.InteractingGameObject.transform;

        if (_interactingObject.gameObject.TryGetComponent(out Collider collider))
        {
            float sizeX = collider.bounds.size.x;
            float sizeY = collider.bounds.size.y;
            float centerY = collider.bounds.center.y;

            _radius = sizeX / 2;
            _heighOffset = centerY - sizeY / 2;
            
            if (_interactingObject.gameObject.TryGetComponent(out NavMeshAgent _))
            {
                _radius += _agentExtraRadius;
            }

            lineRenderer.gameObject.SetActive(true);
            lineRenderer.widthMultiplier = lineWidth;
        }
    }

    private void SetPosition(Vector3 interactingObjectPosition)
    {
        float x;
        float z;
        
        float change = 2 * Mathf.PI / vertexCount;
        float angle = change;

        lineRenderer.positionCount = vertexCount;

        for (int i = 0; i < (vertexCount); i++)
        {
            x = Mathf.Sin(angle) * _radius;
            z = Mathf.Cos(angle) * _radius;

            var pointPosition = new Vector3(x, _heighOffset + _distanceAboveGround, z);
            pointPosition.x += interactingObjectPosition.x;
            pointPosition.z += interactingObjectPosition.z;

            lineRenderer.SetPosition(i, pointPosition);

            angle += change;
        }
    }
}