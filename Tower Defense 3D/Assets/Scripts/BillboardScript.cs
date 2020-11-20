using UnityEngine;

public class BillboardScript : MonoBehaviour
{
    private Transform _cameraTransform;
    private Transform _myTransform;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
        _myTransform = transform;
    }

    private void LateUpdate()
    {
        _myTransform.LookAt(_cameraTransform.forward + _myTransform.position);
    }
}
