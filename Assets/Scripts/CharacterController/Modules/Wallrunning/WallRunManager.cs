using UnityEngine;
using UnityEngine.Events;

public class WallRunManager : MonoBehaviour
{
    public float wallDetectionAngleThreshold = 0.9f;

    public float wallRunAscendingGravity = 1f;
    public float wallRunDescendingGravity = 0.25f;

    public Vector3 WallNormal { get; private set; }

    public bool IsWallRunningOnRightWall => isTouchingWallOnRight && !_groundedManager.IsGrounded;// && !hasWallRunOnRight;
    public bool IsWallRunningOnLeftWall => isTouchingWallOnLeft && !_groundedManager.IsGrounded;// && !hasWallRunOnLeft;
    public bool IsWallRunning => IsWallRunningOnLeftWall || IsWallRunningOnRightWall;

    private bool isTouchingWallOnRight;
    private bool isTouchingWallOnLeft;

    private bool hasWallRunOnRight;
    private bool hasWallRunOnLeft;

    private Vector3 topCollisionPoint;
    private Vector3 middleCollisionPoint;

    private GroundedManager _groundedManager;
    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;










    public float wallRunInitialImpulse = 5f;



    public UnityEvent OnStartedWallRunningRight;
    public UnityEvent OnStartedWallRunningLeft;









    private void Awake()
    {
        _groundedManager = GetComponent<GroundedManager>();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            if (contact.point.y <= topCollisionPoint.y && contact.point.y >= middleCollisionPoint.y)
            {
                var wasWallRunningOnRightWall = IsWallRunningOnRightWall;
                var wasWallRunningOnLeftWall = IsWallRunningOnLeftWall;

                isTouchingWallOnRight = Vector3.Dot(contact.normal, -transform.right) > wallDetectionAngleThreshold;
                isTouchingWallOnLeft = Vector3.Dot(contact.normal, transform.right) > wallDetectionAngleThreshold;

                WallNormal = contact.normal;

                var force = Vector3.zero;

                if (!wasWallRunningOnRightWall && IsWallRunningOnRightWall)
                {
                    OnStartedWallRunningRight?.Invoke();
                    force = Vector3.Cross(-WallNormal, Vector3.up) * wallRunInitialImpulse;
                }

                if (!wasWallRunningOnLeftWall && IsWallRunningOnLeftWall)
                {
                    OnStartedWallRunningLeft?.Invoke();
                    force = Vector3.Cross(WallNormal, Vector3.up) * wallRunInitialImpulse;
                }

                // _rigidbody.AddForce(force, ForceMode.VelocityChange);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (IsWallRunningOnRightWall)
        {
            hasWallRunOnRight = true;
            hasWallRunOnLeft = false;
        }

        if (IsWallRunningOnLeftWall)
        {
            hasWallRunOnLeft = true;
            hasWallRunOnRight = false;
        }

        isTouchingWallOnRight = false;
        isTouchingWallOnLeft = false;

        WallNormal = Vector3.zero;
    }

    private void FixedUpdate()
    {
        Debug.Log($"IsWallRunning: {IsWallRunning}");
        Debug.Log($"IsWallRunningOnRightWall: {IsWallRunningOnRightWall}");
        Debug.Log($"IsWallRunningOnLeftWall: {IsWallRunningOnLeftWall}");

        if (IsWallRunning)
        {
            var wallRunGravity = _rigidbody.linearVelocity.y >= 0 ? wallRunAscendingGravity : wallRunDescendingGravity;

            ApplyWallRunGravity(wallRunGravity);
        }

        if (_groundedManager.IsGrounded)
        {
            hasWallRunOnRight = false;
            hasWallRunOnLeft = false;
        }

        UpdateReferenceCollisionPoints();
    }

    private void ApplyWallRunGravity(float wallRunGravity)
    {
        Vector3 gravity = Physics.gravity * wallRunGravity;
        _rigidbody.AddForce(gravity, ForceMode.Acceleration);
    }

    private void UpdateReferenceCollisionPoints()
    {
        topCollisionPoint = _rigidbody.position + _collider.center + Vector3.up * (_collider.height / 2);
        middleCollisionPoint = _rigidbody.position + _collider.center + Vector3.up * 0.1f;
    }
}
