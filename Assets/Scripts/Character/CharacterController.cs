using UnityEngine;
using UnityEngine.InputSystem;

namespace Character
{
    public class CharacterController : MonoBehaviour
    {
        [SerializeField] private float speed = 10;
        [SerializeField,Range(0,1)] private float mouseSensitivity = 0.5f;
        [SerializeField] private Vector2 yCameraClamp;
        [SerializeField] private Transform cameraPoint;

        private Vector3 _direction;
        private PlayerControls _playerControls;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _playerControls = new PlayerControls();
            
            _playerControls.Player.Movement.performed += OnMovementChanged; 
            _playerControls.Player.Movement.canceled += OnMovementChanged;

            _playerControls.Player.Look.performed += OnMouseDeltaChanged;
            _playerControls.Player.Look.canceled += OnMouseDeltaChanged;

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnMovementChanged(InputAction.CallbackContext obj)
        {
            var direction = obj.ReadValue<Vector2>();
            _direction = new Vector3(direction.x, 0, direction.y);
        }


        private void OnMouseDeltaChanged(InputAction.CallbackContext obj)
        {
            var delta = obj.ReadValue<Vector2>();
            
            transform.eulerAngles += Vector3.up * (delta.x * mouseSensitivity);
            
            float cameraAngle = cameraPoint.eulerAngles.x - (delta.y * mouseSensitivity);
            
            cameraPoint.localEulerAngles = Vector3.right * cameraAngle;
            
        }
        private void FixedUpdate()
        {
            var localDirection = transform.TransformDirection(_direction);
            _rigidbody.velocity = localDirection * (speed);
        }

        private void OnEnable()
        {
            _playerControls.Enable();
        }

        private void OnDisable()
        {
            _playerControls.Disable();
        }
    }
}
