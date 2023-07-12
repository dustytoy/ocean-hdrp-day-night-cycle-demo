using StarterAssets;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
#endif

[RequireComponent(typeof(PlayerInput))]
public class Controller2D : MonoBehaviour
{
    [Header("Player")]
    [Tooltip("Max speed of the character in m/s")]
    public float MaxSpeed = 20f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 5f)]
    public float RotationSmoothTime = 5f;

    [Tooltip("Acceleration and deceleration")]
    //public float SpeedChangeRate = 10.0f;
    public float AccelerationChangeRate = 0.5f;
    public float MaxAcceleration = 5f;

    [Tooltip("Opposite drag force when there is forward speed")]
    public float DragStrength = 1.0f;
    [Tooltip("Sideway steering force")]
    public float SteerStrength = 1.0f;


    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;

    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
    public float CameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]
    public bool LockCameraPosition = false;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    // player
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _speed;
    private float _acceleration;
    private float _drag;
    private Vector3 _velocity;

#if ENABLE_INPUT_SYSTEM
    private PlayerInput _playerInput;
#endif

    private StarterAssetsInputs _input;
    private GameObject _mainCamera;

    private const float _threshold = 0.01f;

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
        }
    }


    private void Awake()
    {
        // get a reference to our main camera
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    private void Start()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

        _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
        _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
    }

    private void Update()
    {
        float delta = Time.deltaTime * DayNightCycle.Instance.timeMultiplier;
        Propulse(delta);
        Drag(delta);
        Steer(delta);
        MoveForward(delta);
    }

    private void LateUpdate()
    {
        CameraRotation(Time.deltaTime * DayNightCycle.Instance.timeMultiplier);
    }

    private void CameraRotation(float deltaTime)
    {
        // if there is an input and camera position is not fixed
        if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            //Don't multiply mouse input by Time.deltaTime;
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : deltaTime;

            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
        }

        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // Cinemachine will follow this target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    private void Propulse(float deltaTime)
    {
        if (_input.move == Vector2.zero)
        {
            _acceleration = 0f;
        }
        else
        {
            if(_input.move.y < 0f)
            {
                _acceleration -= AccelerationChangeRate * deltaTime;
            }
            else if(_input.move.y > 0f)
            {
                _acceleration += AccelerationChangeRate * deltaTime;
            }
        }
        _acceleration = Mathf.Clamp( _acceleration, 0f, MaxAcceleration);
        _speed += _acceleration * deltaTime;
    }
    private void Steer(float deltaTime)
    {
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        if (_input.move != Vector2.zero && _speed > 1f)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg * SteerStrength * _speed / MaxSpeed + _mainCamera.transform.eulerAngles.y;
            var rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);
            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
    }

    private void Drag(float deltaTime)
    {
        if(_speed == 0f)
        {
            _drag = 0f;
            return;
        }
        _drag = DragStrength * Mathf.Clamp(_speed / MaxSpeed, 0.2f, 1f);
        _speed -= _drag * deltaTime;
    }

    private void MoveForward(float deltaTime)
    {
        _speed = Mathf.Clamp(_speed, 0f, MaxSpeed);
        _velocity = transform.forward * _speed;
        transform.Translate(_velocity * deltaTime, Space.World);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        var origin = transform.position + Vector3.up * 30f;
        Gizmos.DrawLine(origin, origin + _velocity);
    }
#endif
}
