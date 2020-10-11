using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float adjustDistanceSpeed;
    [SerializeField] GridPlacementHandler gridHandler;

    private Camera _camera;

    private float _targetDistanceFromGround;
    private float _currentDistanceFromGround;

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        _targetDistanceFromGround = transform.position.y;
        _currentDistanceFromGround = GetCurrentDistanceFromGround();
    }

    void LateUpdate()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            MoveCamera();
            ClampPosition();
        }

        _currentDistanceFromGround = GetCurrentDistanceFromGround();

        if (_targetDistanceFromGround != _currentDistanceFromGround)
        {
            AdjustHeightToTerrain();
        }
    }

    private float GetCurrentDistanceFromGround()
    {
        if (RayCaster.RaycastGameObjectFromCameraCenter(out RaycastHit hitInfo, _camera))
        {
           return transform.position.y - hitInfo.point.y;
        }
        else
        {
            return _targetDistanceFromGround;
        }
    }

    private void MoveCamera()
    {
        float xAxisValue = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float zAxisValue = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        transform.Translate(new Vector3(xAxisValue, 0.0f, zAxisValue), Space.World);
    }

    private void AdjustHeightToTerrain()
    {
        var position = transform.position;

        var diff = _targetDistanceFromGround - _currentDistanceFromGround;

        if (Mathf.Abs(diff) < 0.01f)
        {
            _currentDistanceFromGround = _targetDistanceFromGround;
            return;
        }

        position.y += diff * Time.deltaTime * adjustDistanceSpeed;

        transform.position = position;
    }

    private void ClampPosition()
    {
        var shiftZ = 5;
        var outOfMapDistance = 2;

        var position = transform.position;

        position.x = Mathf.Clamp(position.x, 0 - outOfMapDistance, gridHandler.GridSettings.sizeX + outOfMapDistance);
        position.z = Mathf.Clamp(position.z, 0 - shiftZ - outOfMapDistance, gridHandler.GridSettings.sizeZ - shiftZ + outOfMapDistance);

        transform.position = position;
    }
}
