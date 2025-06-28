using UnityEngine;
using UnityEngine.Events;

public class WallRunManager : MonoBehaviour
{
    public float wallDetectionAngleThreshold = 0.9f;

    public float wallRunAscendingGravity = 1.25f;
    public float wallRunDescendingGravity = 0.75f;

    public float wallStickForce = 10f;

    public float wallRunMinimumSpeed = 10f;

    public UnityEvent OnStartedWallRunningRight;
    public UnityEvent OnStartedWallRunningLeft;

    public Vector3 WallNormal { get; private set; }
    public GameObject WallRunningWall { get; private set; }

    public bool IsMovingForward => _rigidbodyCharacterController.currentInputPayload.MoveInput.y > 0;
    public bool IsWallRunningOnRightWall => isTouchingWallOnRight && !_groundedManager.IsGrounded && IsMovingForward;
    public bool IsWallRunningOnLeftWall => isTouchingWallOnLeft && !_groundedManager.IsGrounded && IsMovingForward;
    public bool IsWallRunning => IsWallRunningOnLeftWall || IsWallRunningOnRightWall;

    private bool isTouchingWallOnRight;
    private bool isTouchingWallOnLeft;

    private Vector3 minimumHeightCollisionPoint;

    private GroundedManager _groundedManager;
    private MovementManager _movementManager;
    private RigidbodyCharacterController _rigidbodyCharacterController;
    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;

    private void Awake()
    {
        _groundedManager = GetComponent<GroundedManager>();
        _movementManager = GetComponent<MovementManager>();
        _rigidbodyCharacterController = GetComponent<RigidbodyCharacterController>();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
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

                WallNormal = contact.normal;

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

        WallNormal = Vector3.zero;
    }

    private void FixedUpdate()
    {
        _movementManager.enabled = !IsWallRunning;

        if (IsWallRunning)
        {
            var wallRunGravity = _rigidbody.linearVelocity.y >= 0 ? wallRunAscendingGravity : wallRunDescendingGravity;

            ApplyWallRunGravity(wallRunGravity);
            ApplyWallStickForce();
            ApplyMinimumSpeed();
        }

        RefreshMinimumHeightCollisionPoint();
    }

    private void ApplyWallStickForce()
    {
        _rigidbody.AddForce(-WallNormal * wallStickForce, ForceMode.Acceleration);
    }

    private void ApplyWallRunGravity(float wallRunGravity)
    {
        Vector3 gravity = Physics.gravity * wallRunGravity;
        _rigidbody.AddForce(gravity, ForceMode.Acceleration);
    }

    private void RefreshMinimumHeightCollisionPoint()
    {
        minimumHeightCollisionPoint = _rigidbody.position + _collider.center + Vector3.up * 0.1f;
    }

    private void ApplyMinimumSpeed()
    {
        if (_rigidbody.linearVelocity.magnitude < wallRunMinimumSpeed)
        {
            var forwardDirectionAlongSideWall = Vector3.ProjectOnPlane(_rigidbody.transform.forward, WallNormal).normalized;

            _rigidbody.linearVelocity = forwardDirectionAlongSideWall * wallRunMinimumSpeed + Vector3.up * _rigidbody.linearVelocity.y;
        }
    }
}
