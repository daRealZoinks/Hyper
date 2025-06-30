using UnityEngine;

public class SlidingModule : MonoBehaviour
{
    public float gravityScale = 2f;
    public float acceleration = 3f;
    public float topSpeed = 0f;
    public float deceleration = 1f;
    public float slidingCapsuleColliderHeight = 1f;
    public Vector3 slidingCapsuleColliderCenter = new(0f, 0.5f, 0f);
    public Vector3 slidingCameraHolderPosition = new(0f, 0.25f, 0f);
    public float cameraLerpSpeed = 10f;

    public bool IsSliding { get; private set; }

    private Vector3 _cameraHolderOriginalPosition;
    private float _capsuleColliderOriginalHeight;
    private Vector3 _capsuleColliderOriginalCenter;

    private Vector3 targetCameraHolderPosition;

    public Transform _cameraHolder;
    private CapsuleCollider _capsuleCollider;
    private Rigidbody _rigidbody;
    private RigidbodyCharacterController _rigidbodyCharacterController;
    private GroundCheckModule _groundCheckModule;
    private MovementModule _movementModule;
    private WallRunModule _wallRunModule;
    private WallJumpModule _wallJumpModule;

    private void Awake()
    {
        _cameraHolderOriginalPosition = _cameraHolder.localPosition;

        _capsuleCollider = GetComponent<CapsuleCollider>();
        _capsuleColliderOriginalHeight = _capsuleCollider.height;
        _capsuleColliderOriginalCenter = _capsuleCollider.center;

        targetCameraHolderPosition = _cameraHolderOriginalPosition;

        _rigidbody = GetComponent<Rigidbody>();
        _rigidbodyCharacterController = GetComponent<RigidbodyCharacterController>();
        _groundCheckModule = GetComponent<GroundCheckModule>();
        _movementModule = GetComponent<MovementModule>();
        _wallRunModule = GetComponent<WallRunModule>();
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

        _cameraHolder.localPosition = Vector3.Lerp(_cameraHolder.localPosition, targetCameraHolderPosition, Time.fixedDeltaTime * cameraLerpSpeed);
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

        if (_groundCheckModule.IsGrounded)
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
                if (_groundCheckModule.GroundNormal == Vector3.up)
                {
                    finalForce *= deceleration;
                }
            }

            if (_groundCheckModule.GroundNormal == Vector3.up)
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
        _movementModule.enabled = false;
        _wallRunModule.enabled = false;
        _wallJumpModule.enabled = false;
        targetCameraHolderPosition = slidingCameraHolderPosition;
        _capsuleCollider.height = slidingCapsuleColliderHeight;
        _capsuleCollider.center = slidingCapsuleColliderCenter;
    }

    private void StopSliding()
    {
        _movementModule.enabled = true;
        _wallRunModule.enabled = true;
        _wallJumpModule.enabled = true;
        targetCameraHolderPosition = _cameraHolderOriginalPosition;
        _capsuleCollider.height = _capsuleColliderOriginalHeight;
        _capsuleCollider.center = _capsuleColliderOriginalCenter;
        IsSliding = false;
    }
}
