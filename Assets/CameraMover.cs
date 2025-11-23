using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;       // 이동 기본 속도
    [SerializeField] private float _boostMultiplier = 3f; // Shift 가속 배율
    [SerializeField] private float _lookSensitivity = 2f; // 마우스 회전 감도
    [SerializeField] private float _speedStep = 2f;       // 휠로 속도 조절 양

    private float _currentSpeed;

    private float _rotationX;
    private float _rotationY;

    private void Start()
    {
        _currentSpeed = _moveSpeed;
        var rot = transform.localRotation.eulerAngles;
        _rotationX = rot.x;
        _rotationY = rot.y;
    }

    private void Update()
    {
        HandleLook();
        HandleMove();
        HandleSpeedChange();
    }

    // 마우스로 바라보는 방향 회전
    private void HandleLook()
    {
        if (Input.GetMouseButton(1)) // 오른쪽 마우스 버튼
        {
            float mouseX = Input.GetAxis("Mouse X") * _lookSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * _lookSensitivity;

            _rotationY += mouseX;
            _rotationX -= mouseY;
            _rotationX = Mathf.Clamp(_rotationX, -89f, 89f);

            transform.localRotation = Quaternion.Euler(_rotationX, _rotationY, 0);
        }
    }

    // 키보드로 이동
    private void HandleMove()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) direction += transform.forward;
        if (Input.GetKey(KeyCode.S)) direction -= transform.forward;
        if (Input.GetKey(KeyCode.A)) direction -= transform.right;
        if (Input.GetKey(KeyCode.D)) direction += transform.right;
        if (Input.GetKey(KeyCode.E)) direction += transform.up;
        if (Input.GetKey(KeyCode.Q)) direction -= transform.up;

        float speed = _currentSpeed;

        // Shift: 가속
        if (Input.GetKey(KeyCode.LeftShift))
            speed *= _boostMultiplier;

        transform.position += direction * speed * Time.deltaTime;
    }

    // 마우스 휠로 속도 조절
    private void HandleSpeedChange()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            _currentSpeed += scroll * _speedStep;
            _currentSpeed = Mathf.Clamp(_currentSpeed, 1f, 50f);
        }
    }
}
