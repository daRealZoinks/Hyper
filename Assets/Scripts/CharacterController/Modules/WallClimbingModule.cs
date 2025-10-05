using UnityEngine;
using UnityEngine.Events;

public class WallClimbingModule : MonoBehaviour
{
    [SerializeField]
    private float wallDetectionAngleThreshold = 0.9f;
    [SerializeField]
    private float wallClimbMaxHeight = 4f;

    public UnityEvent OnStartedWallClimbing;
    public UnityEvent OnStoppedWallClimbing;

    public bool IsWallClimbing { get; private set; } = false;

    public bool IsMovingForward => _rigidbodyCharacterController.CurrentInputPayload.MoveInput.normalized.y > 0.9f;
    public bool CanStartWallClimb => isTouchingWallInFront && !_groundCheckModule.IsGrounded && IsMovingForward && !hasWallClimbedSinceLastNegativeVelocity;

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
            var minimumHeightCollisionPoint = _rigidbody.position + _capsuleCollider.center + Vector3.up * 0.1f;

            if (contact.point.y >= minimumHeightCollisionPoint.y)
            {
                var wasAbleToWallClimb = CanStartWallClimb;

                isTouchingWallInFront = Vector3.Dot(contact.normal, -transform.forward) > wallDetectionAngleThreshold && contact.normal.y == 0;

                if (!wasAbleToWallClimb && CanStartWallClimb && _rigidbody.linearVelocity.y > 0)
                {
                    var climbForce = GetWallClimbAdditiveForce();

                    if (climbForce > 0f)
                    {
                        OnStartedWallClimbing?.Invoke();
                        IsWallClimbing = true;
                        hasWallClimbedSinceLastNegativeVelocity = true;

                        _rigidbody.AddForce(Vector3.up * climbForce, ForceMode.VelocityChange);
                    }
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isTouchingWallInFront = false;

        if (IsWallClimbing)
        {
            OnStoppedWallClimbing?.Invoke();
            IsWallClimbing = false;
        }
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
        if (hasWallClimbedSinceLastNegativeVelocity && _rigidbody.linearVelocity.y < 0f)
        {
            hasWallClimbedSinceLastNegativeVelocity = false;
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
