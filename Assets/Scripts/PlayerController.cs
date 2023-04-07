using Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Footsteps))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Rigs))]

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float WalkSpeed = 1.3f;
    [SerializeField] private float RunSpeed = 4.0f;
    [SerializeField] private float SprintSpeed = 6.0f;
    [SerializeField] private float AimMoveSpeed = 2.0f;
    [SerializeField] private float RotationSmoothTime = 0.12f;
    [SerializeField] private float SpeedChangeRate = 7.5f;
    [SerializeField] private float JumpHeight = 1.2f;

    [SerializeField] private float Gravity = -15.0f;
    [SerializeField] private float JumpTimeout = 0.50f;
    [SerializeField] private float FallTimeout = 0.15f;

    [SerializeField] private GameObject GroundChecker;
    [SerializeField] private bool Grounded = true;
    [SerializeField] private float GroundCheckerRadius = 0.2f;
    [SerializeField] private LayerMask GroundLayers;

    [SerializeField] private GameObject CinemachineCameraTarget;
    [SerializeField] private float TopClamp = 70.0f;
    [SerializeField] private float BottomClamp = -30.0f;
    [SerializeField] private float NormalSensitivity = 1.0f;
    [SerializeField] private float AimSensitivity = 0.5f;

    private CharacterController _controller;
    private PlayerInputManager _input;
    private Animator _animator;
    private GameObject _mainCamera;
    private Gun _gun;
    private Footsteps _footsteps;
    private Rigs _rigs;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private float _sensitivity;
    private const float _threshold = 0.01f;
    [SerializeField] private Transform debugAimTarget;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;

    private float _speed;
    private float _animationBlend;
    private float _verticalBlend;
    private float _horizontalBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private readonly float _terminalVelocity = 53.0f;

    private float _fallTimeoutDelta;
    private float _jumpTimeoutDelta;

    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDAim;
    private int _animIDReload;
    private int _animIDVertical;
    private int _animIDHorizontal;

    private void Awake()
    {
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    private void Start()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputManager>();
        _gun = GetComponentInChildren<Gun>();
        _footsteps = GetComponent<Footsteps>();
        _rigs = GetComponent<Rigs>();

        AssignAnimationIDs();

        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }

    private void Update()
    {
        DoGravity();
        GroundCheck();
        Move();
        Reload();
        Aim();
        Shoot();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void Move()
    {
        float targetSpeed;

        if (!_input.aim)
        {
            if (!_input.sprint)
            {
                targetSpeed = _input.walk ? WalkSpeed : RunSpeed;
            }
            else
            {
                targetSpeed = SprintSpeed;
            }
        }
        else
        {
            targetSpeed = AimMoveSpeed;
        }

        if (_input.move == Vector2.zero) targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed,
                Time.deltaTime * SpeedChangeRate);

            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        _verticalBlend = Mathf.Lerp(_verticalBlend, _input.move.y, Time.deltaTime * SpeedChangeRate);

        _horizontalBlend = Mathf.Lerp(_horizontalBlend, _input.move.x, Time.deltaTime * SpeedChangeRate);

        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        if (_input.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation,
                ref _rotationVelocity, RotationSmoothTime);

            if (!_input.aim || _input.reload)
            {
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                         new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

        _animator.SetFloat(_animIDSpeed, _animationBlend);
        _animator.SetFloat(_animIDVertical, _verticalBlend);
        _animator.SetFloat(_animIDHorizontal, _horizontalBlend);

    }

    private void DoGravity()
    {
        if (Grounded)
        {
            _fallTimeoutDelta = FallTimeout;

            _animator.SetBool(_animIDJump, false);
            _animator.SetBool(_animIDFreeFall, false);

            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            if (_input.jump && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                _animator.SetBool(_animIDJump, true);
            }

            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            _jumpTimeoutDelta = JumpTimeout;

            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _animator.SetBool(_animIDFreeFall, true);
            }

            _input.jump = false;
        }

        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    private void GroundCheck()
    {
        Grounded = Physics.CheckSphere(GroundChecker.transform.position, GroundCheckerRadius,
            GroundLayers, QueryTriggerInteraction.Ignore);

        _animator.SetBool(_animIDGrounded, Grounded);
    }

    private void CameraRotation()
    {
        if (_input.look.sqrMagnitude >= _threshold)
        {
            _cinemachineTargetYaw += _input.look.x * _sensitivity;
            _cinemachineTargetPitch += _input.look.y * _sensitivity;
        }

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch,
            _cinemachineTargetYaw, 0.0f);
    }

    private void Shoot()
    {
        if (_input.reload) return;

        if (_input.aim && _input.shoot)
        {
            _gun.ShootForPlayer(debugAimTarget.position);
        }
    }

    private void Reload()
    {
        if (_input.reload)
        {
            if (_gun.CurrentAmmo == _gun.ClipSize)
            {
                _input.reload = false;
            }
            else
            {
                _rigs.RigsToZero();
                _animator.SetBool(_animIDReload, true);
            }
        }
    }

    private void Aim()
    {
        if (_input.aim && !_input.reload)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            crosshair.SetActive(true);

            _sensitivity = AimSensitivity;

            Vector3 mouseWorldPosition = Vector3.zero;
            Vector3 newDebugAimTarget = Vector3.zero;

            Vector2 screenCenterPoint = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999.0f, aimColliderLayerMask, QueryTriggerInteraction.Ignore))
            {
                newDebugAimTarget = raycastHit.point;
                mouseWorldPosition = raycastHit.point;
            }

            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            debugAimTarget.position = Vector3.Lerp(debugAimTarget.position, newDebugAimTarget, Time.deltaTime * 30.0f);
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20.0f);

            _rigs.RigsToAim();
            _animator.SetBool(_animIDAim, true);
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            crosshair.SetActive(false);

            _sensitivity = NormalSensitivity;

            if (_input.reload) return;

            _rigs.RigsToIdle();
            _animator.SetBool(_animIDAim, false);
        }
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump"); ;
        _animIDFreeFall = Animator.StringToHash("FreeFall"); ;
        _animIDAim = Animator.StringToHash("Aim");
        _animIDReload = Animator.StringToHash("Reload");
        _animIDVertical = Animator.StringToHash("Vertical");
        _animIDHorizontal = Animator.StringToHash("Horizontal");
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnDrawGizmos()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        Gizmos.DrawSphere(GroundChecker.transform.position, GroundCheckerRadius);
    }

    private void OnReloadEnd(AnimationEvent animationEvent)
    {
        _rigs.RigsToOne();
        _gun.Reload();
        _input.reload = false;
        _animator.SetBool(_animIDReload, _input.reload);
    }
    private void OnPlayReloadSound()
    {
        _gun.PlayReloadSound();
    }

    private void OnFootstep()
    {
        _footsteps.PlayFootstepSound();
    }
}