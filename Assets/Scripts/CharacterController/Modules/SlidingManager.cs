using UnityEngine;

public class SlidingManager : MonoBehaviour
{
    public float gravityScale = 1.5f;
    public float acceleration = 3f;
    public float topSpeed = 0f;
    public float deceleration = 1f;
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
    private GroundCheckModule _groundedManager;
    private MovementManager _movementManager;
    private WallRunManager _wallRunManager;
    private WallJumpModule _wallJumpModule;

    private void Awake()
    {
        _cameraHolderOriginalPosition = _cameraHolder.localPosition;

        _capsuleCollider = GetComponent<CapsuleCollider>();
        _capsuleColliderOriginalHeight = _capsuleCollider.height;
        _capsuleColliderOriginalCenter = _capsuleCollider.center;

        _rigidbody = GetComponent<Rigidbody>();
        _rigidbodyCharacterController = GetComponent<RigidbodyCharacterController>();
        _groundedManager = GetComponent<GroundCheckModule>();
        _movementManager = GetComponent<MovementManager>();
        _wallRunManager = GetComponent<WallRunManager>();
        _wallJumpModule = GetComponent<WallJumpModule>();
    }

    private void FixedUpdate()
    {
        if (IsSliding)
        {
            Move(_rigidbodyCharacterController.currentInputPayload.MoveInput);
        }

        if (_rigidbodyCharacterController.currentInputPayload.Sliding && !IsSliding)
        {
            StartSliding();
        }
        else
        {
            if (!_rigidbodyCharacterController.currentInputPayload.Sliding && IsSliding)
            {
                StopSliding();
            }
        }
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

        if (_groundedManager.IsGrounded)
        {
            if (inputDirection != Vector3.zero)
            {
                if (_rigidbody.linearVelocity.magnitude > 0.01f)
                {
                    finalForce *= acceleration;
                }
            }
            else
            {
                if (_groundedManager.GroundNormal == Vector3.up)
                {
                    finalForce *= deceleration;
                }
            }

            if (_groundedManager.GroundNormal == Vector3.up)
            {
                if (_rigidbody.linearVelocity.magnitude > 0.2f)
                {
                    if (!float.IsNaN(finalForce.x) && !float.IsNaN(finalForce.y) && !float.IsNaN(finalForce.z))
                    {
                        _rigidbody.AddForce(finalForce, ForceMode.Acceleration);
                    }
                }
                else
                {
                    _rigidbody.linearVelocity = Vector3.zero;
                }
            }
        }
    }

    private void StartSliding()
    {
        IsSliding = true;
        _movementManager.enabled = false;
        _wallRunManager.enabled = false;
        _wallJumpModule.enabled = false;
        _cameraHolder.localPosition = slidingCameraHolderPosition;
        _capsuleCollider.height = slidingCapsuleColliderHeight;
        _capsuleCollider.center = slidingCapsuleColliderCenter;
    }

    private void StopSliding()
    {
        _movementManager.enabled = true;
        _wallRunManager.enabled = true;
        _wallJumpModule.enabled = true;
        _cameraHolder.localPosition = _cameraHolderOriginalPosition;
        _capsuleCollider.height = _capsuleColliderOriginalHeight;
        _capsuleCollider.center = _capsuleColliderOriginalCenter;
        IsSliding = false;
    }
}
