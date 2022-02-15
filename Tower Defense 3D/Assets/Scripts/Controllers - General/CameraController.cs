using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float adjustDistanceSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] GridController gridController;

    [Header("Camera Zoom")]
    [SerializeField] float zoomSensitivity;
    [SerializeField] float defaultDistanceFromGround;
    [SerializeField] float maxDistanceFromGround;
    [SerializeField] float minDistanceFromGround;

    private Camera _camera;
    private Boundaries _cameraBoundaries;

    private float _targetDistanceFromGround;
    private float _currentDistanceFromGround;

    private void Awake()
    {
        _camera = GetComponentInChildren<Camera>();

        //default camera boundaries to grid size
        var tempBoundaries = new Boundaries();
        tempBoundaries.SetBoundaries(0, gridController.GridSettings.sizeX, 0, gridController.GridSettings.sizeZ);
        SetCameraBoundaries(tempBoundaries);

        _targetDistanceFromGround = defaultDistanceFromGround;
        _currentDistanceFromGround = GetCurrentDistanceFromGround();
    }

    void LateUpdate()
    {
        if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Q))
        {
            RotateCamera();
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0 && Input.GetKey(KeyCode.LeftControl) == false)
        {
            ZoomCamera();
        }

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

    public void SetCameraBoundaries(Boundaries cameraBoundaries)
    {
        _cameraBoundaries = cameraBoundaries;
        ClampPosition();
    }

    public void SetCameraPosition(float x, float z)
    {
        var position = _camera.transform.position;
        position.y = defaultDistanceFromGround;
        _camera.transform.position = position;

        position = transform.position;
        position.x = x;
        position.z = z;
        transform.position = position;
    }

    private void RotateCamera()
    {
        float rotationDirection = 0;

        if (Input.GetKey(KeyCode.E))
            rotationDirection = 1;
        
        if (Input.GetKey(KeyCode.Q))
            rotationDirection = -1;

        transform.Rotate(Vector3.up, rotationDirection * rotationSpeed * GetBaseDeltaTime());
    }

    private float GetCurrentDistanceFromGround()
    {
        return _camera.transform.position.y;

        //if (RayCaster.RaycastGameObjectWithTagFromCameraCenter(out RaycastHit hitInfo, _camera, "Terrain"))
        //{
        //   return _camera.transform.position.y - hitInfo.point.y;
        //}
        //else
        //{
        //    return _camera.transform.position.y;
        //}
    }

    private void MoveCamera()
    {
        float xAxisValue = Input.GetAxis("Horizontal") * moveSpeed * GetBaseDeltaTime();
        float zAxisValue = Input.GetAxis("Vertical") * moveSpeed * GetBaseDeltaTime();

        var moveVector = new Vector3(xAxisValue, 0.0f, zAxisValue);

        transform.Translate(Quaternion.AngleAxis(transform.rotation.eulerAngles.y, Vector3.up) * moveVector, Space.World);
    }

    private void AdjustHeightToTerrain()
    {
        var position = _camera.transform.position;

        if (Mathf.Approximately(_targetDistanceFromGround, _currentDistanceFromGround))
        {
            _currentDistanceFromGround = _targetDistanceFromGround;
            return;
        }

        var diff = _targetDistanceFromGround - _currentDistanceFromGround;

        position.y += diff * adjustDistanceSpeed * GetBaseDeltaTime();

        _camera.transform.position = position;
    }

    private void ClampPosition()
    {
        var outOfMapDistance = 2;

        var position = transform.position;

        position.x = Mathf.Clamp(position.x, _cameraBoundaries.X1 - outOfMapDistance, _cameraBoundaries.X2 + outOfMapDistance);
        position.z = Mathf.Clamp(position.z, _cameraBoundaries.Z1 - outOfMapDistance, _cameraBoundaries.Z2 + outOfMapDistance);

        transform.position = position;
    }

    private void ZoomCamera()
    {
        float zoomValue = Input.GetAxis("Mouse ScrollWheel") * -1000;

        _targetDistanceFromGround += zoomSensitivity * zoomValue * GetBaseDeltaTime();

        _targetDistanceFromGround = Mathf.Clamp(_targetDistanceFromGround, minDistanceFromGround, maxDistanceFromGround);
    }

    private float GetBaseDeltaTime()
    {
        float timeScale = Time.timeScale == 0 ? 1 : Time.timeScale;

        return Time.deltaTime / timeScale;
    }
}
