using UnityEngine;
using UnityEngine.Events;

public class WallClimbingModule : MonoBehaviour
{
    public float wallDetectionAngleThreshold = 0.9f;
    public float wallClimbMaxHeight = 4f;

    public bool IsMovingForward => _rigidbodyCharacterController.CurrentInputPayload.MoveInput.normalized.y > 0.9f;
    public bool IsWallClimbing => isTouchingWallInFront && !_groundCheckModule.IsGrounded && IsMovingForward && !hasWallClimbedSinceLastNegativeVelocity;

    public UnityEvent OnStartedWallClimbing;

    private Vector3 minimumHeightCollisionPoint;

    private bool isTouchingWallInFront;
    private bool hasWallClimbedSinceLastNegativeVelocity = false;

    private RigidbodyCharacterController _rigidbodyCharacterController;
    private GravityModule _gravityModule;
    private GroundCheckModule _groundCheckModule;
    private GroundJumpModule _groundJumpModule;
    private Rigidbody _rigidbody;
    private CapsuleCollider _capsuleCollider;

    private void Awake()
    {
        _rigidbodyCharacterController = GetComponent<RigidbodyCharacterController>();
        _gravityModule = GetComponent<GravityModule>();
        _groundCheckModule = GetComponent<GroundCheckModule>();
        _groundJumpModule = GetComponent<GroundJumpModule>();
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

                if (!wasWallClimbing && isTouchingWallInFront && !_groundCheckModule.IsGrounded && IsMovingForward && !hasWallClimbedSinceLastNegativeVelocity)
                {
                    float forceToAdd = GetWallClimbAdditiveForce();
                    if (forceToAdd > 0f)
                    {
                        OnStartedWallClimbing?.Invoke();
                        hasWallClimbedSinceLastNegativeVelocity = true;
                        _rigidbody.linearVelocity = new Vector3(
                            _rigidbody.linearVelocity.x,
                            _rigidbody.linearVelocity.y + forceToAdd,
                            _rigidbody.linearVelocity.z
                        );
                    }
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isTouchingWallInFront = false;
    }

    private float GetWallClimbAdditiveForce()
    {
        var upwardsVelocity = _rigidbody.linearVelocity.y;
        var gravity = Physics.gravity.y * _gravityModule.defaultGravityScale;
        var currentAirHeight = _groundJumpModule.jumpHeight - Mathf.Pow(upwardsVelocity, 2) / (2 * -gravity);

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

    private void FixedUpdate()
    {
        RefreshMinimumHeightCollisionPoint();

        if (hasWallClimbedSinceLastNegativeVelocity && _rigidbody.linearVelocity.y < 0f)
        {
            hasWallClimbedSinceLastNegativeVelocity = false;
        }
    }

    private void RefreshMinimumHeightCollisionPoint()
    {
        minimumHeightCollisionPoint = _rigidbody.position + _capsuleCollider.center + Vector3.up * 0.1f;
    }
}
