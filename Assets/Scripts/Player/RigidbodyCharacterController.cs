using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Hyper.Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class RigidbodyCharacterController : MonoBehaviour
    {
        [field: Header("Movement Settings")]
        [field: SerializeField] public float Acceleration { get; private set; } = 90f;
        [field: SerializeField] public float TopSpeed { get; private set; } = 13f;
        [field: SerializeField] public float Deceleration { get; private set; } = 180f;
        [field: SerializeField] public float AirControl { get; private set; } = 0.25f;
        [field: SerializeField] public float AirBreak { get; private set; } = 0f;

        [field: Header("Jump Settings")]
        [field: SerializeField] public float JumpHeight { get; private set; } = 2f;
        [field: SerializeField] public float CoyoteTime { get; private set; } = 0.15f;
        [field: SerializeField] public float JumpBufferTime { get; private set; } = 0.15f;

        [field: Header("Wall Running Settings")]
        [field: SerializeField] public float WallRunDetectionAngleThreshold { get; private set; } = 0.9f;
        [field: SerializeField] public float WallStickForce { get; private set; } = 7f;
        [field: SerializeField] public float WallRunLandBoost { get; private set; } = 2f;
        [field: SerializeField] public float WallRunGravityResistanceForce { get; private set; } = 10f;

        [field: Header("Wall Jump Settings")]
        [field: SerializeField] public float WallJumpAngleFromWallNormal { get; private set; } = 25f;
        [field: SerializeField] public float WallJumpHeight { get; private set; } = 1.5f;
        [field: SerializeField] public float WallJumpBoost { get; private set; } = 1f;

        [field: Header("Sliding Settings")]
        [field: SerializeField] public float SlidingDownForce { get; private set; } = 5f;
        [field: SerializeField] public float SlidingTurnSpeed { get; private set; } = 0.2f;
        [field: SerializeField] public float SlidingCapsuleColliderHeight { get; private set; } = 1f;
        [field: SerializeField] public Vector3 SlidingCapsuleColliderCenter { get; private set; } = new(0f, 0.5f, 0f);
        [field: SerializeField] public Vector3 SlidingCameraTrackingTargetPosition { get; private set; } = new(0f, 0.5f, 0f);

        [field: Header("Mantling and Wall Climbing Settings")]
        [field: SerializeField] public float FrontWallDetectionAngleThreshold { get; private set; } = 0.9f;
        [field: SerializeField] public float MaxMantleDuration { get; private set; } = 0.35f;
        [field: SerializeField] public float MantleBoost { get; private set; } = 2f;
        [field: SerializeField] public float WallClimbMaxHeight { get; private set; } = 4f;

        [field: Header("General Settings")]
        [field: SerializeField] public bool UseGravity { get; set; } = true;
        [field: SerializeField] public float GravityScale { get; private set; } = 1.5f;
        [field: SerializeField] public float SlopeLimit { get; private set; } = 45f;
        [field: SerializeField] public LayerMask GroundCheckLayerMask { get; private set; }

        [Header("Events")]
        public UnityEvent<float> OnLanded;
        public UnityEvent OnJump;

        public UnityEvent OnStartedWallRunningRight;
        public UnityEvent OnStartedWallRunningLeft;

        public UnityEvent OnStoppedWallrunning;

        public UnityEvent OnRightWallJump;
        public UnityEvent OnLeftWallJump;

        public UnityEvent OnMantle;

        public UnityEvent OnWallClimb;

        [Header("References")]
        [SerializeField] private new Camera camera;
        [SerializeField] private Transform cameraTrackingTarget;

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

        public bool IsWallRunningOnRightWall => _isWallRunningOnRightWall;
        public bool _isWallRunningOnRightWall;
        public bool IsWallRunningOnLeftWall => _isWallRunningOnLeftWall;
        public bool _isWallRunningOnLeftWall;

        public bool IsWallRunning => _isWallRunningOnLeftWall || _isWallRunningOnRightWall;

        public bool IsSliding { get; private set; }

        public bool IsMantling { get; private set; }

        public bool IsWallClimbing => _isTouchingWallInFront && !IsGrounded && _rigidbody.linearVelocity.y > 0f;

        // private variables
        private Vector3 groundNormal;

        private float _jumpBufferCounter;
        private float _coyoteTimeCounter;

        private bool _isTouchingWallOnRight;
        private bool _isTouchingWallOnLeft;

        private Vector3 _wallContactNormal;
        private Collider _wallRunningWall;
        private Collider _lastWallRunningWall;

        private float _capsuleColliderOriginalHeight;
        private Vector3 _capsuleColliderOriginalCenter;
        private Vector3 _cameraTrackingTargetOriginalPosition;

        private bool _isTouchingWallInFront;
        private bool _hasWallClimbedSinceLastNegativeVelocity = false;
        private Vector3 _linearVelocityBeforeClimb;

        private Rigidbody _rigidbody;
        private CapsuleCollider _capsuleCollider;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _capsuleCollider = GetComponent<CapsuleCollider>();
        }

        private void Start()
        {
            _cameraTrackingTargetOriginalPosition = cameraTrackingTarget.localPosition;

            _capsuleColliderOriginalHeight = _capsuleCollider.height;
            _capsuleColliderOriginalCenter = _capsuleCollider.center;
        }

        private void FixedUpdate()
        {
            GroundCheck();

            UpdateRotationBasedOnCamera();

            UpdateJumpBufferCounter();
            UpdateCoyoteTimeCounter();

            WallRunCheck();

            WallClimbCheck();

            UpdateSlidingState();

            if (!IsMantling)
            {
                ApplyCustomGravity(GravityScale);
            }

            if (IsGrounded)
            {
                _wallRunningWall = null;
                _lastWallRunningWall = null;
            }

            if (!IsGrounded && !IsSliding && IsMovingForward)
            {
                MantleCheck();
            }

            if (!IsSliding)
            {
                Move(MoveInput);
            }

            if (!IsSliding)
            {
                HandleJumpLogic();
            }

            if (!IsSliding && IsWallRunning)
            {
                ApplyWallRunGravityResistanceForce();
                ApplyWallStickForce();
            }
        }

        private void GroundCheck()
        {
            const float radiusOffset = 0.1f;
            const float maxDistanceOffset = 0.15f;

            var ray = new Ray(_rigidbody.position + _capsuleCollider.center, Vector3.down);
            var radius = _capsuleCollider.radius - radiusOffset;
            var maxDistance = _capsuleCollider.height / 2 - _capsuleCollider.radius + maxDistanceOffset;

            var results = new RaycastHit[5];

            var sphereCastHitsCount = Physics.SphereCastNonAlloc(ray, radius, results, maxDistance, GroundCheckLayerMask);

            if (sphereCastHitsCount <= 0)
            {
                IsGrounded = false;
                return;
            }

            var validHits = results
                    .Take(sphereCastHitsCount)
                    .Where(r => r.collider != null)
                    .Where(r => r.point.y < (_rigidbody.position.y + _capsuleCollider.radius / Mathf.Sqrt(2)))
                    .ToArray();

            if (validHits.Length == 0)
            {
                IsGrounded = false;
                return;
            }

            var closestHitToCenter = validHits.OrderBy(r => Vector3.Distance(r.point, _rigidbody.position)).First();

            var groundNormalAngle = Vector3.Angle(closestHitToCenter.normal, Vector3.up);

            if (groundNormalAngle > SlopeLimit)
            {
                IsGrounded = false;
                return;
            }

            ray = new Ray(closestHitToCenter.point + Vector3.up, Vector3.down);

            sphereCastHitsCount = Physics.RaycastNonAlloc(ray, results, 2f, GroundCheckLayerMask);

            if (sphereCastHitsCount <= 0)
            {
                IsGrounded = false;
                return;
            }

            var raycastHit = results
                .Take(sphereCastHitsCount)
                .Where(r => r.collider != null)
                .ToArray()
                .First();

            var isOnSlope = raycastHit.normal == closestHitToCenter.normal;

            if (isOnSlope)
            {
                groundNormal = closestHitToCenter.normal;
            }
            else
            {
                groundNormal = _rigidbody.position + _capsuleCollider.radius * Vector3.up - closestHitToCenter.point;
            }

            if (!IsGrounded)
            {
                IsGrounded = true;
                OnLanded?.Invoke(Mathf.Abs(_rigidbody.linearVelocity.y));
                _coyoteTimeCounter = CoyoteTime;
            }
        }

        private void MantleCheck()
        {
            var upperBoxCastHitsResults = new RaycastHit[5];
            var lowerBoxCastHitsResults = new RaycastHit[5];

            var upperBoxCastHitsNumber = Physics.BoxCastNonAlloc(_rigidbody.position + _capsuleCollider.center + Vector3.up * 0.5f, new Vector3(0.25f, 0.25f, 0.25f), transform.forward, upperBoxCastHitsResults, Quaternion.LookRotation(transform.forward), _capsuleCollider.radius + 0.25f, GroundCheckLayerMask);
            var lowerBoxCastHitsNumber = Physics.BoxCastNonAlloc(_rigidbody.position + Vector3.up * _capsuleCollider.radius, new Vector3(0.25f, 0.1f, 0.25f), transform.forward, lowerBoxCastHitsResults, Quaternion.LookRotation(transform.forward), _capsuleCollider.radius + 0.25f, GroundCheckLayerMask);

            if (upperBoxCastHitsNumber == 0 && lowerBoxCastHitsNumber > 0)
            {
                var validHits = lowerBoxCastHitsResults
                    .Take(lowerBoxCastHitsNumber)
                    .Where(r => r.collider != null)
                    .ToArray();

                var closestHitToCenter = validHits.OrderBy(r => Vector3.Distance(r.point, _rigidbody.position)).First();

                var hasHitRightAngleSurface = closestHitToCenter.normal.y == 0;

                if (hasHitRightAngleSurface)
                {
                    var positionOfMantlingRay = new Ray(_rigidbody.position + _capsuleCollider.center + Vector3.up * _capsuleCollider.height + (_capsuleCollider.radius + 0.5f) * transform.forward, Vector3.down);

                    var positionOfMantlingRaycastHits = new RaycastHit[5];

                    var positionOfMantlingRaycastHitsNumber = Physics.RaycastNonAlloc(positionOfMantlingRay, positionOfMantlingRaycastHits, 2f, GroundCheckLayerMask);

                    Debug.DrawLine(positionOfMantlingRay.origin, positionOfMantlingRay.origin + positionOfMantlingRay.direction * 2, Color.red, 2f);

                    RaycastHit hit;

                    if (positionOfMantlingRaycastHitsNumber > 0)
                    {
                        validHits = positionOfMantlingRaycastHits
                           .Take(positionOfMantlingRaycastHitsNumber)
                           .Where(r => r.collider != null)
                           .ToArray();

                        var tallestPoint = validHits.OrderByDescending(r => r.point.y).First();

                        hit = tallestPoint;
                    }
                    else
                    {
                        hit = new RaycastHit
                        {
                            point = positionOfMantlingRay.origin + Vector3.down * 2f
                        };
                    }

                    if (!IsMantling)
                    {
                        Mantle(hit, closestHitToCenter.normal, _rigidbody.linearVelocity);
                    }

                    OnMantle?.Invoke();
                }
            }
        }

        private void WallClimbCheck()
        {
            var checkRay = new Ray(_rigidbody.position + _capsuleCollider.center + Vector3.up * 0.1f, transform.forward);

            var raycastHitsNumber = Physics.RaycastNonAlloc(checkRay, new RaycastHit[1], _capsuleCollider.radius + 0.1f, GroundCheckLayerMask);

            _isTouchingWallInFront = raycastHitsNumber > 0;

            if (_isTouchingWallInFront && _rigidbody.linearVelocity.y > 0f && !_hasWallClimbedSinceLastNegativeVelocity && !IsGrounded && !IsSliding && IsMovingForward)
            {
                var climbForce = GetWallClimbAdditiveForce();

                if (climbForce > 0f)
                {
                    OnWallClimb?.Invoke();
                    _hasWallClimbedSinceLastNegativeVelocity = true;

                    _linearVelocityBeforeClimb = _rigidbody.linearVelocity;

                    _rigidbody.AddForce(Vector3.up * climbForce, ForceMode.VelocityChange);
                }
            }

            if (_hasWallClimbedSinceLastNegativeVelocity && _rigidbody.linearVelocity.y < 0f)
            {
                _hasWallClimbedSinceLastNegativeVelocity = false;
            }
        }

        private void WallRunCheck()
        {
            WallRunCheck(ref _isWallRunningOnRightWall, transform.right, ref _isTouchingWallOnRight, OnStartedWallRunningLeft);
            WallRunCheck(ref _isWallRunningOnLeftWall, -transform.right, ref _isTouchingWallOnLeft, OnStartedWallRunningLeft);
        }

        private void WallRunCheck(ref bool isWallRunningOnWall, Vector3 direction, ref bool isTouchingWall, UnityEvent onStartedWallrunning)
        {
            var ray = new Ray(_rigidbody.position + _capsuleCollider.center, direction);
            var maxDistance = _capsuleCollider.radius + 0.1f;

            var results = new RaycastHit[5];

            var raycastHitsCount = Physics.RaycastNonAlloc(ray, results, maxDistance, GroundCheckLayerMask);

            var validHits = results
                    .Take(raycastHitsCount)
                    .Where(r => r.collider != null)
                    .ToArray();

            var wasWallRunningOnWall = isWallRunningOnWall;

            isTouchingWall = validHits.Length > 0;

            if (isTouchingWall && !IsGrounded && !IsMantling && IsVelocityForward && MoveInput.normalized.y >= 0.70f)
            {
                var closestHitToCenter = validHits.OrderBy(r => Vector3.Distance(r.point, _rigidbody.position + _capsuleCollider.center)).First();

                _wallRunningWall = closestHitToCenter.collider;
                _wallContactNormal = closestHitToCenter.normal;

                if (!wasWallRunningOnWall
                    && (!_lastWallRunningWall || _wallRunningWall != _lastWallRunningWall))
                {
                    isWallRunningOnWall = true;

                    var forwardDirectionAlongSideWall = Vector3.ProjectOnPlane(transform.forward, _wallContactNormal).normalized;
                    _rigidbody.AddForce(forwardDirectionAlongSideWall * WallRunLandBoost, ForceMode.VelocityChange);

                    onStartedWallrunning?.Invoke();
                }
            }
            else
            {
                if (wasWallRunningOnWall)
                {
                    isWallRunningOnWall = false;
                    _lastWallRunningWall = _wallRunningWall;
                    _wallRunningWall = null;

                    OnStoppedWallrunning?.Invoke();
                }
            }
        }

        private void ApplyCustomGravity(float gravityScale)
        {
            var gravitationalForceDirection = IsGrounded ? -groundNormal : Vector3.down;
            var gravitationalForceMagnitude = Mathf.Abs(Physics.gravity.y) * gravityScale;
            _rigidbody.AddForce(gravitationalForceDirection * gravitationalForceMagnitude, ForceMode.Acceleration);
        }

        public void Jump()
        {
            _jumpBufferCounter = JumpBufferTime;
        }

        private void UpdateRotationBasedOnCamera()
        {
            var cameraYaw = camera.transform.rotation.eulerAngles.y;
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

            var horizontalClampedVelocity = horizontalRigidbodyVelocity.normalized * Mathf.Clamp01(horizontalRigidbodyVelocity.magnitude / TopSpeed);

            var finalForce = inputDirection - horizontalClampedVelocity;

            finalForce *= inputDirection != Vector3.zero ? Acceleration : Deceleration;

            if (IsGrounded)
            {
                finalForce = Vector3.ProjectOnPlane(finalForce, groundNormal);
            }
            else
            {
                finalForce *= inputDirection != Vector3.zero ? AirControl : AirBreak;
            }

            _rigidbody.AddForce(finalForce, ForceMode.Acceleration);
        }

        private void HandleJumpLogic()
        {
            if (_jumpBufferCounter > 0f)
            {
                if (_coyoteTimeCounter > 0f || IsGrounded)
                {
                    GroundJump();
                    return;
                }

                if (IsWallRunning && _wallRunningWall)
                {
                    WallJump();
                }
            }
        }

        private void ApplyWallRunGravityResistanceForce()
        {
            if (_rigidbody.linearVelocity.y < 0)
            {
                _rigidbody.AddForce(Vector3.up * WallRunGravityResistanceForce, ForceMode.Acceleration);
            }
        }

        private void ApplyWallStickForce()
        {
            _rigidbody.AddForce(-_wallContactNormal * WallStickForce, ForceMode.Acceleration);
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
            ExecuteWallJump();

            if (_isWallRunningOnRightWall)
            {
                OnRightWallJump?.Invoke();
            }

            if (_isWallRunningOnLeftWall)
            {
                OnLeftWallJump?.Invoke();
            }

            _jumpBufferCounter = 0f;
        }

        private void ExecuteGroundJump()
        {
            var jumpForce = Vector3.up * Mathf.Sqrt(-2f * Physics.gravity.y * GravityScale * JumpHeight);

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
            var jumpForce = Vector3.up * Mathf.Sqrt(-2 * Physics.gravity.y * GravityScale * WallJumpHeight);

            var normalized = Vector3.ProjectOnPlane(transform.forward, _wallContactNormal).normalized;

            var vector3 = Vector3.Slerp(normalized, _wallContactNormal, WallJumpAngleFromWallNormal / 90f);

            var playerHorizontalMagnitude = new Vector3()
            {
                x = _rigidbody.linearVelocity.x,
                z = _rigidbody.linearVelocity.z
            }.magnitude;

            var playerNewVelocity = vector3 * (playerHorizontalMagnitude + WallJumpBoost);

            _rigidbody.linearVelocity = playerNewVelocity + jumpForce;
        }

        private void UpdateSlidingState()
        {
            if (Sliding)
            {
                if (!IsSliding)
                {
                    StartSliding();
                }

                _rigidbody.AddForce(Vector3.down * (SlidingDownForce + (IsGrounded ? -Physics.gravity.y * GravityScale : 0)), ForceMode.Acceleration);

                var horizontalVelocity = new Vector3
                {
                    x = _rigidbody.linearVelocity.x,
                    z = _rigidbody.linearVelocity.z
                };

                var desiredDirection = (transform.right * MoveInput.x + transform.forward * MoveInput.y).normalized;
                var projectedDesiredDirection = Vector3.ProjectOnPlane(desiredDirection, groundNormal).normalized;
                var newHorizontalVelocity = Vector3.Slerp(horizontalVelocity.normalized, projectedDesiredDirection, MoveInput.magnitude * SlidingTurnSpeed * Time.fixedDeltaTime) * horizontalVelocity.magnitude;

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
            cameraTrackingTarget.localPosition = SlidingCameraTrackingTargetPosition;
            _capsuleCollider.height = SlidingCapsuleColliderHeight;
            _capsuleCollider.center = SlidingCapsuleColliderCenter;
        }

        private void StopSliding()
        {
            cameraTrackingTarget.localPosition = _cameraTrackingTargetOriginalPosition;
            _capsuleCollider.height = _capsuleColliderOriginalHeight;
            _capsuleCollider.center = _capsuleColliderOriginalCenter;
            IsSliding = false;
        }

        private async void Mantle(RaycastHit raycastHit, Vector3 forwardWallNormal, Vector3 linearVelocity)
        {
            var currentAirHeight = CurrentAirHeightBasedOnVelocity(linearVelocity);

            var isWallClimbingBeforeMantle = IsWallClimbing;

            IsMantling = true;
            _capsuleCollider.enabled = false;
            _rigidbody.linearVelocity = Vector3.zero;

            var mantleElapsedTime = 0f;
            var mantleStart = transform.position;
            var mantleEnd = raycastHit.point;
            var distance = (mantleEnd - mantleStart).magnitude;
            var mantleDuration = Mathf.Min(distance / linearVelocity.magnitude, MaxMantleDuration);

            while (mantleElapsedTime < mantleDuration)
            {
                mantleElapsedTime += Time.deltaTime;
                var t = Mathf.Clamp01(mantleElapsedTime / mantleDuration);
                transform.position = Vector3.Lerp(mantleStart, mantleEnd, t);
                await Task.Yield();
            }

            IsMantling = false;
            _capsuleCollider.enabled = true;

            if (isWallClimbingBeforeMantle)
            {
                var oldHorizontalLinearVelocity = new Vector3
                {
                    x = _linearVelocityBeforeClimb.x,
                    z = _linearVelocityBeforeClimb.z
                };

                if (currentAirHeight < 0)
                {
                    _rigidbody.linearVelocity = -forwardWallNormal * (oldHorizontalLinearVelocity.magnitude + MantleBoost);
                }
                else
                {
                    _rigidbody.linearVelocity = -forwardWallNormal * oldHorizontalLinearVelocity.magnitude;
                }
            }
            else
            {
                var oldHorizontalLinearVelocity = new Vector3
                {
                    x = linearVelocity.x,
                    z = linearVelocity.z
                };

                _rigidbody.linearVelocity = -forwardWallNormal * (oldHorizontalLinearVelocity.magnitude + MantleBoost);
            }

            transform.position = mantleEnd;
        }

        private float GetWallClimbAdditiveForce()
        {
            var currentAirHeight = CurrentAirHeightBasedOnVelocity(_rigidbody.linearVelocity);

            if (currentAirHeight < 0)
            {
                currentAirHeight = 0;
            }

            var heightDifference = WallClimbMaxHeight - currentAirHeight;

            if (heightDifference > 0)
            {
                var upwardForce = Mathf.Sqrt(2 * -(Physics.gravity.y * GravityScale) * heightDifference);
                var forceToAdd = upwardForce - _rigidbody.linearVelocity.y;
                return forceToAdd > 0f ? forceToAdd : 0f;
            }

            return 0f;
        }

        private float CurrentAirHeightBasedOnVelocity(Vector3 linearVelocity)
        {
            var upwardsVelocity = linearVelocity.y;
            var gravity = Physics.gravity.y * GravityScale;
            return JumpHeight - Mathf.Pow(upwardsVelocity, 2) / (2 * -gravity);
        }
    }
}