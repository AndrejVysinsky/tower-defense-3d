﻿using UnityEngine;

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

    private float _targetDistanceFromGround;
    private float _currentDistanceFromGround;

    private void Awake()
    {
        _camera = GetComponentInChildren<Camera>();

        //_camera.transform.LookAt(transform.position);
        
        var position = _camera.transform.position;
        position.y = maxDistanceFromGround;
        _camera.transform.position = position;

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

    private void RotateCamera()
    {
        float rotationDirection = 0;

        if (Input.GetKey(KeyCode.E))
            rotationDirection = 1;
        
        if (Input.GetKey(KeyCode.Q))
            rotationDirection = -1;

        transform.Rotate(Vector3.up, rotationDirection * rotationSpeed * Time.deltaTime / Time.timeScale);
    }

    private float GetCurrentDistanceFromGround()
    {
        if (RayCaster.RaycastGameObjectWithTagFromCameraCenter(out RaycastHit hitInfo, _camera, "Terrain"))
        {
           return _camera.transform.position.y - hitInfo.point.y;
        }
        else
        {
            return _camera.transform.position.y;
        }
    }

    private void MoveCamera()
    {
        float xAxisValue = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime / Time.timeScale;
        float zAxisValue = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime / Time.timeScale;

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

        position.y += diff * Time.deltaTime * adjustDistanceSpeed;

        _camera.transform.position = position;
    }

    private void ClampPosition()
    {
        var outOfMapDistance = 2;

        var position = transform.position;

        position.x = Mathf.Clamp(position.x, 0 - outOfMapDistance, gridController.GridSettings.sizeX + outOfMapDistance);
        position.z = Mathf.Clamp(position.z, 0 - outOfMapDistance, gridController.GridSettings.sizeZ + outOfMapDistance);

        transform.position = position;
    }

    private void ZoomCamera()
    {
        float zoomValue = Input.GetAxis("Mouse ScrollWheel") * -1000;

        _targetDistanceFromGround += zoomSensitivity * zoomValue * Time.deltaTime;

        _targetDistanceFromGround = Mathf.Clamp(_targetDistanceFromGround, minDistanceFromGround, maxDistanceFromGround);
    }
}
