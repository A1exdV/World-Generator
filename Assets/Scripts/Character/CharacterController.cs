using UnityEngine;
using UnityEngine.InputSystem;

namespace Character
{
    public class CharacterController : MonoBehaviour
    {
        [SerializeField] private float speed = 10;
        [SerializeField] private float rotationSpeed = 10;

        private Vector3 _direction;
        private PlayerControls _playerControls;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _playerControls = new PlayerControls();
            
            _playerControls.Player.Movement.performed += OnMovementChanged; 
            _playerControls.Player.Movement.canceled += OnMovementChanged;

            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnMovementChanged(InputAction.CallbackContext obj)
        {
            var direction = obj.ReadValue<Vector2>();
            _direction = new Vector3(direction.x, 0, direction.y);
        }

        private void Update()
        {
            if(_rigidbody.velocity == Vector3.zero) return;
            Quaternion targetRotation = Quaternion.LookRotation(_rigidbody.velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        private void FixedUpdate()
        {
            ApplyPlayerVelocity();
        }

        private void ApplyPlayerVelocity()
        {
            _rigidbody.velocity = _direction * (speed);
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
