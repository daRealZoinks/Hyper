using UnityEngine;
using UnityEngine.Events;

public class WallClimbingManager : MonoBehaviour
{
    public float wallDetectionAngleThreshold = 0.9f;
    public float wallClimbMaxHeight = 4f;

    public bool IsMovingForward => _rigidbodyCharacterController.currentInputPayload.MoveInput.y > 0;
    public bool IsWallClimbing => isTouchingWallInFront && !_groundedManager.IsGrounded && IsMovingForward;

    public UnityEvent OnStartedWallClimbing;

    private Vector3 minimumHeightCollisionPoint;

    private bool isTouchingWallInFront;

    private RigidbodyCharacterController _rigidbodyCharacterController;
    private GravityModule _gravityModule;
    private GroundedManager _groundedManager;
    private GroundJumpManager _groundJumpManager;
    private Rigidbody _rigidbody;
    private CapsuleCollider _capsuleCollider;

    private void Awake()
    {
        _rigidbodyCharacterController = GetComponent<RigidbodyCharacterController>();
        _gravityModule = GetComponent<GravityModule>();
        _groundedManager = GetComponent<GroundedManager>();
        _groundJumpManager = GetComponent<GroundJumpManager>();
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            if (contact.point.y >= minimumHeightCollisionPoint.y)
            {
                var wasWallClimbing = IsWallClimbing;

                isTouchingWallInFront = Vector3.Dot(contact.normal, -transform.forward) > wallDetectionAngleThreshold && contact.normal.y == 0;

                if (!wasWallClimbing && IsWallClimbing)
                {
                    OnStartedWallClimbing?.Invoke();
                    if (_rigidbody.linearVelocity.y > 0)
                    {
                        ApplyWallClimbUpwardForce();
                    }
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isTouchingWallInFront = false;
    }

    private void ApplyWallClimbUpwardForce()
    {
        var upwardsVelocity = _rigidbody.linearVelocity.y;
        var gravity = Physics.gravity.y * _gravityModule.gravityScale;
        var currentAirHeight = _groundJumpManager.jumpHeight - Mathf.Pow(upwardsVelocity, 2) / (2 * -gravity);

        if (currentAirHeight < 0)
        {
            currentAirHeight = 0;
        }

        var heightDifference = wallClimbMaxHeight - currentAirHeight;

        if (heightDifference > 0)
        {
            var upwardForce = Mathf.Sqrt(2 * -gravity * heightDifference);

            _rigidbody.linearVelocity = new Vector3
            {
                x = _rigidbody.linearVelocity.x,
                y = upwardForce,
                z = _rigidbody.linearVelocity.z
            };
        }
    }

    private void FixedUpdate()
    {
        RefreshMinimumHeightCollisionPoint();
    }

    private void RefreshMinimumHeightCollisionPoint()
    {
        minimumHeightCollisionPoint = _rigidbody.position + _capsuleCollider.center + Vector3.up * 0.1f;
    }
}
