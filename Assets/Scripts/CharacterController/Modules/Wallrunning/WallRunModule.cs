using UnityEngine;
using UnityEngine.Events;

public class WallRunModule : MonoBehaviour
{
    [SerializeField]
    private float wallDetectionAngleThreshold = 0.9f;
    [SerializeField]
    private float wallStickForce = 10f;
    [SerializeField]
    private float wallRunMinimumSpeed = 10f;

    public float wallRunAscendingGravity = 1.25f;
    public float wallRunDescendingGravity = 0.75f;

    public UnityEvent OnStartedWallRunningRight;
    public UnityEvent OnStartedWallRunningLeft;

    public ContactPoint WallContactPoint { get; private set; }
    public GameObject WallRunningWall { get; private set; }

    public bool IsMovingForward => _rigidbodyCharacterController.CurrentInputPayload.MoveInput.normalized.y > 0.7f;
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

    public bool IsWallRunningOnRightWall => isTouchingWallOnRight && !_groundCheckModule.IsGrounded && IsMovingForward && IsVelocityForward;
    public bool IsWallRunningOnLeftWall => isTouchingWallOnLeft && !_groundCheckModule.IsGrounded && IsMovingForward && IsVelocityForward;
    public bool IsWallRunning => IsWallRunningOnLeftWall || IsWallRunningOnRightWall;

    private bool isTouchingWallOnRight;
    private bool isTouchingWallOnLeft;

    private Vector3 minimumHeightCollisionPoint;

    private GroundCheckModule _groundCheckModule;
    private MovementModule _movementModule;
    private RigidbodyCharacterController _rigidbodyCharacterController;
    private Rigidbody _rigidbody;
    private CapsuleCollider _capsuleCollider;

    private void Awake()
    {
        _groundCheckModule = GetComponent<GroundCheckModule>();
        _movementModule = GetComponent<MovementModule>();
        _rigidbodyCharacterController = GetComponent<RigidbodyCharacterController>();
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            if (contact.point.y >= minimumHeightCollisionPoint.y)
            {
                var wasWallRunningOnRightWall = IsWallRunningOnRightWall;
                var wasWallRunningOnLeftWall = IsWallRunningOnLeftWall;

                isTouchingWallOnRight = Vector3.Dot(contact.normal, -transform.right) > wallDetectionAngleThreshold;
                isTouchingWallOnLeft = Vector3.Dot(contact.normal, transform.right) > wallDetectionAngleThreshold;

                WallContactPoint = contact;

                if (!wasWallRunningOnRightWall && IsWallRunningOnRightWall)
                {
                    OnStartedWallRunningRight?.Invoke();
                    WallRunningWall = collision.gameObject;
                }

                if (!wasWallRunningOnLeftWall && IsWallRunningOnLeftWall)
                {
                    OnStartedWallRunningLeft?.Invoke();
                    WallRunningWall = collision.gameObject;
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isTouchingWallOnRight = false;
        isTouchingWallOnLeft = false;

        WallContactPoint = new ContactPoint();
    }

    private void FixedUpdate()
    {
        _movementModule.enabled = !IsWallRunning;

        if (IsWallRunning)
        {
            ApplyWallStickForce();
            ApplyMinimumSpeed();
        }

        RefreshMinimumHeightCollisionPoint();
    }

    private void ApplyWallStickForce()
    {
        _rigidbody.AddForce(-WallContactPoint.normal * wallStickForce, ForceMode.Acceleration);
    }

    private void RefreshMinimumHeightCollisionPoint()
    {
        minimumHeightCollisionPoint = _rigidbody.position + _capsuleCollider.center + Vector3.up * 0.1f;
    }

    private void ApplyMinimumSpeed()
    {
        if (_rigidbody.linearVelocity.magnitude < wallRunMinimumSpeed)
        {
            var forwardDirectionAlongSideWall = Vector3.ProjectOnPlane(_rigidbody.transform.forward, WallContactPoint.normal).normalized;

            _rigidbody.linearVelocity = forwardDirectionAlongSideWall * wallRunMinimumSpeed + Vector3.up * _rigidbody.linearVelocity.y;
        }
    }
}
