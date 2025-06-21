using UnityEngine;
using UnityEngine.Events;

public class WallRunManager : MonoBehaviour
{
    public float wallDetectionAngleThreshold = 0.9f;

    public float wallRunAscendingGravity = 1f;
    public float wallRunDescendingGravity = 0.25f;

    public UnityEvent OnStartedWallRunningRight;
    public UnityEvent OnStartedWallRunningLeft;

    public Vector3 WallNormal { get; private set; }

    public bool IsWallRunningOnRightWall => isTouchingWallOnRight && !_groundedManager.IsGrounded;
    public bool IsWallRunningOnLeftWall => isTouchingWallOnLeft && !_groundedManager.IsGrounded;
    public bool IsWallRunning => IsWallRunningOnLeftWall || IsWallRunningOnRightWall;

    private bool isTouchingWallOnRight;
    private bool isTouchingWallOnLeft;

    private Vector3 minimumHeightCollisionPoint;

    private GroundedManager _groundedManager;
    private MovementManager _movementManager;
    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;


    private void Awake()
    {
        _groundedManager = GetComponent<GroundedManager>();
        _movementManager = GetComponent<MovementManager>();
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
                }

                if (!wasWallRunningOnLeftWall && IsWallRunningOnLeftWall)
                {
                    OnStartedWallRunningLeft?.Invoke();
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
        }

        RefreshMinimumHeightCollisionPoint();
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
}
