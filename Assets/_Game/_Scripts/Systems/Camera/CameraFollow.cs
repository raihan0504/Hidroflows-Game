using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] Vector3 _offset;
    [SerializeField] float _smoothSpeed = 5f;

    private void LateUpdate()
    {
        Vector3 desiredPosition = _target.position + _offset;

        transform.position = Vector3.Lerp(
            transform.position, 
            desiredPosition, 
            _smoothSpeed * Time.deltaTime);
    }
}