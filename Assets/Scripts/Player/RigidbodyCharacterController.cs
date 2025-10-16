using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyCharacterController : MonoBehaviour
{
    // public variables
    public float acceleration = 60f;
    public float topSpeed = 8f;
    public float deceleration = 120f;

    public float airControl = 0.25f;
    public float airBreak = 0f;

    public float jumpHeight = 2f;
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.15f;

    public float wallDetectionAngleThreshold = 0.9f;
    public float wallStickForce = 10f;
    public float wallRunMinimumSpeed = 10f;
    public float wallRunAscendingForce = 3f;
    public float wallRunDescendingForce = 10f;

    public float wallJumpHeight = 1.5f;
    public float wallJumpSideForce = 4f;
    public float wallJumpForwardForce = 5f;
    public float sameWallJumpCooldown = 2.5f;




    public float gravityScale = 1.5f;
    public float slopeLimit = 45f;

    // public events
    public UnityEvent OnLanded;
    public UnityEvent OnJump;

    public UnityEvent OnStartedWallRunningRight;
    public UnityEvent OnStartedWallRunningLeft;

    public UnityEvent OnRightWallJump;
    public UnityEvent OnLeftWallJump;

    // private references to other objects
    [SerializeField]
    private Camera _camera;

    // public properties
    public Vector2 MoveInput { private get; set; }
    public bool Sliding { private get; set; }

    public bool IsMovingForward => MoveInput.normalized.y > 0.7f;
    public bool IsVelocityForward
    {
        get
        {
            var horizontalVelocity = new Vector3
            {
                x = _rigidbody.linearVelocity.x,
                z = _rigidbody.linearVelocity.z
            };

            return Vector3.Dot(horizontalVelocity.normalized, transform.forward) > 0.7f;
        }
    }

    public bool IsWallRunningOnRightWall => _isTouchingWallOnRight && !isGrounded && IsMovingForward && IsVelocityForward;
    public bool IsWallRunningOnLeftWall => _isTouchingWallOnLeft && !isGrounded && IsMovingForward && IsVelocityForward;
    public bool IsWallRunning => IsWallRunningOnLeftWall || IsWallRunningOnRightWall;


    // private variables
    private bool isGrounded;
    private Vector3 groundNormal;

    private float _jumpBufferCounter;
    private float _coyoteTimeCounter;

    private bool _isTouchingWallOnRight;
    private bool _isTouchingWallOnLeft;

    private ContactPoint _wallContactPoint;
    private GameObject _wallRunningWall;
    private GameObject _lastWallJumped;

    private float _sameWallJumpCooldownCounter;

    // private references to components
    private Rigidbody _rigidbody;
    private CapsuleCollider _capsuleCollider;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void FixedUpdate()
    {
        if (isGrounded)
        {
            _lastWallJumped = null;
            _sameWallJumpCooldownCounter = 0f;
        }
        else
        {
            ApplyCustomGravity(gravityScale);
        }

        UpdateRotationBasedOnCamera();

        if (!IsWallRunning)
        {
            Move(MoveInput);
        }

        HandleJumpLogic();

        if (IsWallRunning)
        {
            ApplyWallRunForce();
            ApplyWallStickForce();
            ApplyMinimumSpeed();
        }

        UpdateSameWallJumpCooldownCounter();
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (var contactPoint in collision.contacts)
        {
            var angle = Vector3.Angle(contactPoint.normal, Vector3.up);

            if (angle <= slopeLimit)
            {
                if (!isGrounded)
                {
                    isGrounded = true;
                    groundNormal = contactPoint.normal;
                    OnLanded?.Invoke();
                    _coyoteTimeCounter = coyoteTime;
                    break;
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (var contactPoint in collision.contacts)
        {
            var angle = Vector3.Angle(contactPoint.normal, Vector3.up);

            if (angle <= slopeLimit)
            {
                isGrounded = true;
                groundNormal = contactPoint.normal;
                break;
            }

            var minimumHeightCollisionPoint = _rigidbody.position + _capsuleCollider.center + Vector3.up * 0.1f;

            if (contactPoint.point.y >= minimumHeightCollisionPoint.y)
            {
                var wasWallRunningOnRightWall = IsWallRunningOnRightWall;
                var wasWallRunningOnLeftWall = IsWallRunningOnLeftWall;

                _isTouchingWallOnRight = Vector3.Dot(contactPoint.normal, -transform.right) > wallDetectionAngleThreshold;
                _isTouchingWallOnLeft = Vector3.Dot(contactPoint.normal, transform.right) > wallDetectionAngleThreshold;

                _wallContactPoint = contactPoint;

                if (!wasWallRunningOnRightWall && IsWallRunningOnRightWall)
                {
                    OnStartedWallRunningRight?.Invoke();
                    _wallRunningWall = collision.gameObject;
                }

                if (!wasWallRunningOnLeftWall && IsWallRunningOnLeftWall)
                {
                    OnStartedWallRunningLeft?.Invoke();
                    _wallRunningWall = collision.gameObject;
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;

        _isTouchingWallOnRight = false;
        _isTouchingWallOnLeft = false;

        _wallContactPoint = new ContactPoint();
    }

    private void ApplyCustomGravity(float gravityScale)
    {
        _rigidbody.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);
    }

    public void Jump()
    {
        _jumpBufferCounter = jumpBufferTime;
    }

    private void UpdateRotationBasedOnCamera()
    {
        var cameraForward = _camera.transform.forward;
        cameraForward.y = 0;
        _rigidbody.rotation = Quaternion.LookRotation(cameraForward.normalized);
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

        finalForce *= (inputDirection != Vector3.zero) ? acceleration : deceleration;

        if (isGrounded)
        {
            finalForce = Vector3.ProjectOnPlane(finalForce, groundNormal);
        }
        else
        {
            // TODO: make better 1 - default transitions between airControl and airBreak 
            finalForce *= inputDirection != Vector3.zero ? airControl : airBreak;
        }

        _rigidbody.AddForce(finalForce, ForceMode.Acceleration);
    }

    private void HandleJumpLogic()
    {
        UpdateJumpBufferCounter();
        UpdateCoyoteTimeCounter();

        if (_jumpBufferCounter > 0f)
        {
            if (_coyoteTimeCounter > 0f || isGrounded)
            {
                GroundJump();
            }

            if (!isGrounded && IsWallRunning)
            {
                WallJump();
            }
        }
    }

    private void ApplyWallRunForce()
    {
        var upwardForce = _rigidbody.linearVelocity.y < 0 ? wallRunDescendingForce : wallRunAscendingForce;
        _rigidbody.AddForce(Vector3.up * upwardForce, ForceMode.Acceleration);
    }

    private void ApplyWallStickForce()
    {
        _rigidbody.AddForce(-_wallContactPoint.normal * wallStickForce, ForceMode.Acceleration);
    }

    private void ApplyMinimumSpeed()
    {
        if (_rigidbody.linearVelocity.magnitude < wallRunMinimumSpeed)
        {
            var forwardDirectionAlongSideWall = Vector3.ProjectOnPlane(_rigidbody.transform.forward, _wallContactPoint.normal).normalized;

            _rigidbody.linearVelocity = forwardDirectionAlongSideWall * wallRunMinimumSpeed + Vector3.up * _rigidbody.linearVelocity.y;
        }
    }

    private void UpdateSameWallJumpCooldownCounter()
    {
        if (_sameWallJumpCooldownCounter > 0f)
        {
            _sameWallJumpCooldownCounter -= Time.fixedDeltaTime;
        }
    }

    private void UpdateJumpBufferCounter()
    {
        if (_jumpBufferCounter > 0f)
        {
            _jumpBufferCounter -= Time.fixedDeltaTime;
        }
    }

    private void UpdateCoyoteTimeCounter()
    {
        if (!isGrounded)
        {
            if (_coyoteTimeCounter > 0f)
            {
                _coyoteTimeCounter -= Time.fixedDeltaTime;
            }
        }
    }

    private void GroundJump()
    {
        ExecuteGroundJump();
        OnJump?.Invoke();
        _coyoteTimeCounter = 0f;
        _jumpBufferCounter = 0f;
    }

    private void WallJump()
    {
        if ((!_wallRunningWall || _wallRunningWall != _lastWallJumped || sameWallJumpCooldown <= 0f) && IsWallRunning)
        {
            _lastWallJumped = _wallRunningWall;
            _sameWallJumpCooldownCounter = sameWallJumpCooldown;
            ExecuteWallJump();

            if (IsWallRunningOnRightWall)
            {
                OnRightWallJump?.Invoke();
            }

            if (IsWallRunningOnLeftWall)
            {
                OnLeftWallJump?.Invoke();
            }
        }
    }

    private void ExecuteGroundJump()
    {
        var jumpForce = Vector3.up * Mathf.Sqrt(-2f * Physics.gravity.y * gravityScale * jumpHeight);

        if (_rigidbody.linearVelocity.y < 0)
        {
            _rigidbody.linearVelocity = new Vector3()
            {
                x = _rigidbody.linearVelocity.x,
                z = _rigidbody.linearVelocity.z
            };
        }

        _rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);
    }

    private void ExecuteWallJump()
    {
        var sideForce = _wallContactPoint.normal * wallJumpSideForce;
        var jumpForce = Vector3.up * Mathf.Sqrt(-2 * Physics.gravity.y * gravityScale * wallJumpHeight);
        var forwardForce = transform.forward * wallJumpForwardForce;

        var finalForce = sideForce + jumpForce + forwardForce;

        _rigidbody.linearVelocity = new Vector3()
        {
            x = _rigidbody.linearVelocity.x,
            z = _rigidbody.linearVelocity.z
        };

        _rigidbody.AddForce(finalForce, ForceMode.VelocityChange);
    }
}
