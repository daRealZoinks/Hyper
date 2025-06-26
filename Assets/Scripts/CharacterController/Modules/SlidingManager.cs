using UnityEngine;

public class SlidingManager : MonoBehaviour
{
    public float gravityScale = 1.5f;
    public float acceleration = 6f;
    public float topSpeed = 1f;
    public float deceleration = 12f;
    public float slidingCapsuleColliderHeight = 1f;
    public Vector3 slidingCapsuleColliderCenter = new(0f, 0.5f, 0f);
    public Vector3 slidingCameraHolderPosition = new(0f, 0.25f, 0f);

    public bool IsSliding { get; private set; }

    private Vector3 _cameraHolderOriginalPosition;
    private float _capsuleColliderOriginalHeight;
    private Vector3 _capsuleColliderOriginalCenter;

    public Transform _cameraHolder;
    private CapsuleCollider _capsuleCollider;
    private Rigidbody _rigidbody;
    private RigidbodyCharacterController _rigidbodyCharacterController;
    private GroundedManager _groundedManager;
    private MovementManager _movementManager;
    private JumpManager _jumpManager;
    private WallRunManager _wallRunManager;

    private void Awake()
    {
        _cameraHolderOriginalPosition = _cameraHolder.localPosition;

        _capsuleCollider = GetComponent<CapsuleCollider>();
        _capsuleColliderOriginalHeight = _capsuleCollider.height;
        _capsuleColliderOriginalCenter = _capsuleCollider.center;

        _rigidbody = GetComponent<Rigidbody>();
        _rigidbodyCharacterController = GetComponent<RigidbodyCharacterController>();
        _groundedManager = GetComponent<GroundedManager>();
        _jumpManager = GetComponent<JumpManager>();
        _movementManager = GetComponent<MovementManager>();
        _wallRunManager = GetComponent<WallRunManager>();
    }

    private void FixedUpdate()
    {
        if (IsSliding)
        {
            ApplyCustomGravity(gravityScale);
            Move(_rigidbodyCharacterController.currentInputPayload.MoveInput);
        }

        if (_rigidbodyCharacterController.currentInputPayload.Sliding && !IsSliding)
        {
            StartSliding();
            IsSliding = true;
        }
        else
        {
            if (!_rigidbodyCharacterController.currentInputPayload.Sliding && IsSliding)
            {
                StopSliding();
                IsSliding = false;
            }
        }
    }

    private void ApplyCustomGravity(float gravityScale)
    {
        _rigidbody.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);
    }

    private void Move(Vector2 moveInput)
    {
        var inputDirection = transform.right * moveInput.x + transform.forward * moveInput.y;

        var horizontalRigidbodyVelocity = new Vector3
        {
            x = _rigidbody.linearVelocity.x,
            z = _rigidbody.linearVelocity.z
        };

        var horizontalClampedVelocity = horizontalRigidbodyVelocity.normalized * Mathf.Clamp01(horizontalRigidbodyVelocity.magnitude / topSpeed);

        var finalForce = inputDirection - horizontalClampedVelocity;

        finalForce *= (inputDirection != Vector3.zero || _groundedManager.GroundNormal != Vector3.up) ? acceleration : deceleration;

        _rigidbody.AddForce(finalForce, ForceMode.Acceleration);
    }

    private void StartSliding()
    {
        _movementManager.enabled = false;
        _jumpManager.enabled = false;
        _wallRunManager.enabled = false;
        _cameraHolder.localPosition = slidingCameraHolderPosition;
        _capsuleCollider.height = slidingCapsuleColliderHeight;
        _capsuleCollider.center = slidingCapsuleColliderCenter;
    }

    private void StopSliding()
    {
        _movementManager.enabled = true;
        _jumpManager.enabled = true;
        _wallRunManager.enabled = true;
        _cameraHolder.localPosition = _cameraHolderOriginalPosition;
        _capsuleCollider.height = _capsuleColliderOriginalHeight;
        _capsuleCollider.center = _capsuleColliderOriginalCenter;
    }
}
