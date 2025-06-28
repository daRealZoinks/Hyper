using UnityEngine;
using UnityEngine.Events;

public class WallClimbingManager : MonoBehaviour
{
    public float wallDetectionAngleThreshold = 0.9f;

    public bool IsMovingForward => _rigidbodyCharacterController.currentInputPayload.MoveInput.y > 0;
    public bool IsWallClimbing => isTouchingWallInFront && !_groundedManager.IsGrounded && IsMovingForward;

    public UnityEvent OnStartedWallClimbing;

    private Vector3 minimumHeightCollisionPoint;

    private bool isTouchingWallInFront;

    private RigidbodyCharacterController _rigidbodyCharacterController;
    private GroundedManager _groundedManager;
    private Rigidbody _rigidbody;
    private CapsuleCollider _capsuleCollider;

    private void Awake()
    {
        _rigidbodyCharacterController = GetComponent<RigidbodyCharacterController>();
        _groundedManager = GetComponent<GroundedManager>();
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

                isTouchingWallInFront = Vector3.Dot(contact.normal, -transform.forward) > wallDetectionAngleThreshold;

                if (!wasWallClimbing && IsWallClimbing)
                {
                    OnStartedWallClimbing?.Invoke();
                    // push player upwards based on how high up he is
                    // calculate how high up he is based on his velocity
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isTouchingWallInFront = false;
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
