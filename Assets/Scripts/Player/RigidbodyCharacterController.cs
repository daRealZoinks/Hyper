using UnityEngine;
using UnityEngine.Events;

namespace Hyper.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyCharacterController : MonoBehaviour
    {
        // public variables
        [Header("Movement Settings")]
        public float acceleration = 60f;
        public float topSpeed = 8f;
        public float deceleration = 120f;

        public float airControl = 0.25f;
        public float airBreak = 0f;

        [Header("Jump Settings")]
        public float jumpHeight = 2f;
        public float coyoteTime = 0.15f;
        public float jumpBufferTime = 0.15f;

        [Header("Wall Running Settings")]
        public float wallRunDetectionAngleThreshold = 0.9f;
        public float wallStickForce = 10f;
        public float wallRunMinimumSpeed = 10f;
        public float wallRunAscendingForce = 3f;
        public float wallRunDescendingForce = 10f;

        [Header("Wall Jump Settings")]
        public float wallJumpHeight = 1.5f;
        public float wallJumpSideForce = 4f;
        public float wallJumpForwardForce = 5f;
        public float sameWallJumpCooldown = 2.5f;

        [Header("Sliding Settings")]
        public float slidingDownForce = 5f;
        public float slidingTurnSpeed = 0.2f;
        public float slidingCapsuleColliderHeight = 1f;
        public Vector3 slidingCapsuleColliderCenter = new(0f, 0.5f, 0f);
        public Vector3 slidingCameraTrackingTargetPosition = new(0f, 0.5f, 0f);

        [Header("Mantling and Wall Climbing Settings")]
        public float frontWallDetectionAngleThreshold = 0.9f;
        public float mantleDuration = 0.2f;
        public float wallClimbMaxHeight = 4f;

        [Header("General Settings")]
        public float gravityScale = 1.5f;
        public float slopeLimit = 45f;

        // public events
        [Header("Events")]
        public UnityEvent OnLanded;
        public UnityEvent OnJump;

        public UnityEvent OnStartedWallRunningRight;
        public UnityEvent OnStartedWallRunningLeft;

        public UnityEvent OnRightWallJump;
        public UnityEvent OnLeftWallJump;

        public UnityEvent OnMantle;

        public UnityEvent OnStartedWallClimbing;
        public UnityEvent OnStoppedWallClimbing;

        // private references to other objects
        [Header("References")]
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private Transform _cameraTrackingTarget;

        // public properties
        public Vector2 MoveInput { private get; set; }
        public bool Sliding { private get; set; }

        public bool IsMovingForward => MoveInput.normalized.y > 0.9f;
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

        public bool IsGrounded { get; set; }

        public bool IsWallRunningOnRightWall => _isTouchingWallOnRight && !IsGrounded && IsMovingForward && IsVelocityForward;
        public bool IsWallRunningOnLeftWall => _isTouchingWallOnLeft && !IsGrounded && IsMovingForward && IsVelocityForward;
        public bool IsWallRunning => IsWallRunningOnLeftWall || IsWallRunningOnRightWall;

        public bool IsSliding { get; private set; }

        public bool IsMantling { get; private set; }

        public bool IsWallClimbing { get; private set; } = false;
        public bool CanStartWallClimb => _wallClimbFrontWallDetection && !IsGrounded && IsMovingForward && !_hasWallClimbedSinceLastNegativeVelocity;

        private Vector3 groundNormal;

        private float _jumpBufferCounter;
        private float _coyoteTimeCounter;

        private bool _isTouchingWallOnRight;
        private bool _isTouchingWallOnLeft;

        private ContactPoint _wallContactPoint;
        private GameObject _wallRunningWall;

        private GameObject _lastWallJumped;
        private float _sameWallJumpCooldownCounter;

        private float _capsuleColliderOriginalHeight;
        private Vector3 _capsuleColliderOriginalCenter;
        private Vector3 _cameraTrackingTargetOriginalPosition;

        private bool _mantlingFrontWallDetection;
        private Vector3 _mantleStart;
        private Vector3 _mantleEnd;
        private float _mantleElapsedTime;

        private bool _wallClimbFrontWallDetection;
        private bool _hasWallClimbedSinceLastNegativeVelocity = false;

        // private references to components
        private Rigidbody _rigidbody;
        private CapsuleCollider _capsuleCollider;

        private void Awake()
        {
            _cameraTrackingTargetOriginalPosition = _cameraTrackingTarget.localPosition;

            _rigidbody = GetComponent<Rigidbody>();

            _capsuleCollider = GetComponent<CapsuleCollider>();
            _capsuleColliderOriginalHeight = _capsuleCollider.height;
            _capsuleColliderOriginalCenter = _capsuleCollider.center;
        }

        private void Update()
        {
            UpdateMantlingState();
        }

        private void FixedUpdate()
        {
            if (IsGrounded)
            {
                _lastWallJumped = null;
                _sameWallJumpCooldownCounter = 0f;
            }
            else
            {
                if (!IsMantling)
                {
                    ApplyCustomGravity(gravityScale);
                }
            }

            UpdateRotationBasedOnCamera();

            if (!IsWallRunning && !IsSliding)
            {
                Move(MoveInput);
            }

            HandleJumpLogic();

            if (!IsSliding)
            {
                if (IsWallRunning)
                {
                    ApplyWallRunForce();
                    ApplyWallStickForce();
                    ApplyMinimumSpeed();
                }
            }

            UpdateSameWallJumpCooldownCounter();

            UpdateSlidingState();

            UpdateWallClimbingState();
        }

        private void OnCollisionEnter(Collision collision)
        {
            foreach (var contactPoint in collision.contacts)
            {
                var angle = Vector3.Angle(contactPoint.normal, Vector3.up);

                if (angle <= slopeLimit)
                {
                    if (!IsGrounded)
                    {
                        IsGrounded = true;
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
                    IsGrounded = true;
                    groundNormal = contactPoint.normal;
                    break;
                }
            }

            ContactPoint? touchingBelowMaximumHeight = null;
            ContactPoint? touchingAboveMaximumHeight = null;

            foreach (var contactPoint in collision.contacts)
            {
                var minimumHeightCollisionPoint = _rigidbody.position + _capsuleCollider.center + Vector3.up * 0.1f;

                if (contactPoint.point.y >= minimumHeightCollisionPoint.y)
                {
                    // TODO: cant tell if wall running detection just doesnt work or the threshold is too high

                    var wasWallRunningOnRightWall = IsWallRunningOnRightWall;
                    var wasWallRunningOnLeftWall = IsWallRunningOnLeftWall;

                    var wasAbleToWallClimb = CanStartWallClimb;

                    _isTouchingWallOnRight = Vector3.Dot(contactPoint.normal, -transform.right) > frontWallDetectionAngleThreshold;
                    _isTouchingWallOnLeft = Vector3.Dot(contactPoint.normal, transform.right) > frontWallDetectionAngleThreshold;

                    _wallContactPoint = contactPoint;

                    _wallClimbFrontWallDetection = Vector3.Dot(contactPoint.normal, -transform.forward) > frontWallDetectionAngleThreshold && contactPoint.normal.y == 0;

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

                    if (!wasAbleToWallClimb && CanStartWallClimb && _rigidbody.linearVelocity.y > 0)
                    {
                        var climbForce = GetWallClimbAdditiveForce();

                        if (climbForce > 0f)
                        {
                            OnStartedWallClimbing?.Invoke();
                            IsWallClimbing = true;
                            _hasWallClimbedSinceLastNegativeVelocity = true;

                            _rigidbody.AddForce(Vector3.up * climbForce, ForceMode.VelocityChange);
                        }
                    }
                }

                _mantlingFrontWallDetection = Vector3.Dot(contactPoint.normal, -transform.forward) > frontWallDetectionAngleThreshold && contactPoint.normal.y == 0;

                if (_mantlingFrontWallDetection && !IsGrounded && IsMovingForward && !IsSliding)
                {
                    var maximumHeightCollisionPoint = _rigidbody.position + _capsuleCollider.center + Vector3.up * 0.1f;

                    if (contactPoint.point.y <= maximumHeightCollisionPoint.y)
                    {
                        touchingBelowMaximumHeight = contactPoint;
                    }
                    else
                    {
                        touchingAboveMaximumHeight = contactPoint;
                    }
                }
            }

            if (touchingBelowMaximumHeight != null && touchingAboveMaximumHeight == null)
            {
                OnMantle?.Invoke();

                Mantle(touchingBelowMaximumHeight.Value);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            IsGrounded = false; // TODO: not changing if whatever is under the player vanishes

            _isTouchingWallOnRight = false;
            _isTouchingWallOnLeft = false;

            _wallContactPoint = new ContactPoint();

            _wallClimbFrontWallDetection = false;

            if (IsWallClimbing)
            {
                OnStoppedWallClimbing?.Invoke();
                IsWallClimbing = false;
            }
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
            var cameraYaw = _camera.transform.rotation.eulerAngles.y;
            _rigidbody.rotation = Quaternion.Euler(0f, cameraYaw, 0f);
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

            finalForce *= inputDirection != Vector3.zero ? acceleration : deceleration;

            if (IsGrounded)
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

            if (!IsSliding)
            {
                if (_jumpBufferCounter > 0f)
                {
                    if (_coyoteTimeCounter > 0f || IsGrounded)
                    {
                        // TODO: Jump is being triggered multiple times, fix

                        GroundJump();
                    }

                    if (!IsGrounded && IsWallRunning)
                    {
                        WallJump();
                    }
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
            if (!IsGrounded)
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

        private void UpdateSlidingState()
        {
            if (Sliding)
            {
                if (!IsSliding)
                {
                    StartSliding();
                }

                _rigidbody.AddForce(Vector3.down * (slidingDownForce + (IsGrounded ? -Physics.gravity.y * gravityScale : 0)), ForceMode.Acceleration);

                var horizontalVelocity = new Vector3
                {
                    x = _rigidbody.linearVelocity.x,
                    z = _rigidbody.linearVelocity.z
                };

                var desiredDirection = (transform.right * MoveInput.x + transform.forward * MoveInput.y).normalized;
                var projectedDesiredDirection = Vector3.ProjectOnPlane(desiredDirection, groundNormal).normalized;
                var newHorizontalVelocity = Vector3.Slerp(horizontalVelocity.normalized, projectedDesiredDirection, MoveInput.magnitude * slidingTurnSpeed * Time.fixedDeltaTime) * horizontalVelocity.magnitude;

                _rigidbody.linearVelocity = new Vector3
                {
                    x = newHorizontalVelocity.x,
                    y = _rigidbody.linearVelocity.y,
                    z = newHorizontalVelocity.z
                };
            }
            else
            {
                if (IsSliding)
                {
                    StopSliding();
                }
            }
        }

        private void StartSliding()
        {
            IsSliding = true;
            _cameraTrackingTarget.localPosition = slidingCameraTrackingTargetPosition;
            _capsuleCollider.height = slidingCapsuleColliderHeight;
            _capsuleCollider.center = slidingCapsuleColliderCenter;
        }

        private void StopSliding()
        {
            _cameraTrackingTarget.localPosition = _cameraTrackingTargetOriginalPosition;
            _capsuleCollider.height = _capsuleColliderOriginalHeight;
            _capsuleCollider.center = _capsuleColliderOriginalCenter;
            IsSliding = false;
        }

        private void UpdateMantlingState()
        {
            if (IsMantling)
            {
                _capsuleCollider.enabled = false;
                _rigidbody.linearVelocity = Vector3.zero;
                _mantleElapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(_mantleElapsedTime / mantleDuration);
                transform.position = Vector3.Lerp(_mantleStart, _mantleEnd, t);

                if (_mantleElapsedTime >= mantleDuration)
                {
                    transform.position = _mantleEnd;
                    IsMantling = false;
                }
            }
            else
            {
                _capsuleCollider.enabled = true;
            }
        }

        private void Mantle(ContactPoint touchingBelowMaximumHeight)
        {
            if (IsMantling) return;

            var capsuleColliderCenterPosition = transform.position;
            var mantleForwardOffset = transform.forward * _capsuleCollider.radius;
            var mantleVerticalOffset = transform.up * (touchingBelowMaximumHeight.point.y - capsuleColliderCenterPosition.y);

            _mantleStart = capsuleColliderCenterPosition;
            _mantleEnd = _mantleStart + mantleForwardOffset + mantleVerticalOffset;
            _mantleElapsedTime = 0f;

            IsMantling = true;
        }

        private float GetWallClimbAdditiveForce()
        {
            var upwardsVelocity = _rigidbody.linearVelocity.y;
            var gravity = Physics.gravity.y * gravityScale;
            var currentAirHeight = jumpHeight - Mathf.Pow(upwardsVelocity, 2) / (2 * -gravity);

            if (currentAirHeight < 0)
            {
                currentAirHeight = 0;
            }

            var heightDifference = wallClimbMaxHeight - currentAirHeight;

            if (heightDifference > 0)
            {
                var upwardForce = Mathf.Sqrt(2 * -gravity * heightDifference);
                var forceToAdd = upwardForce - upwardsVelocity;
                return forceToAdd > 0f ? forceToAdd : 0f;
            }

            return 0f;
        }

        private void UpdateWallClimbingState()
        {
            if (_hasWallClimbedSinceLastNegativeVelocity && _rigidbody.linearVelocity.y < 0f)
            {
                _hasWallClimbedSinceLastNegativeVelocity = false;
            }

            if (IsWallClimbing)
            {
                if (_rigidbody.linearVelocity.y < 0f)
                {
                    OnStoppedWallClimbing?.Invoke();
                    IsWallClimbing = false;
                }
            }
        }
    }
}