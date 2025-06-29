using UnityEngine;
using UnityEngine.Events;

public class MantleManager : MonoBehaviour
{
    public float wallDetectionAngleThreshold = 0.9f;

    public bool IsMovingForward => _rigidbodyCharacterController.currentInputPayload.MoveInput.y > 0;
    public UnityEvent OnMantle;

    private Vector3 maximumHeightCollisionPoint;

    private bool isTouchingWallInFront;

    private RigidbodyCharacterController _rigidbodyCharacterController;
    private MovementManager _movementManager;
    private GroundedManager _groundedManager;
    private SlidingManager _slidingManager;
    private Rigidbody _rigidbody;
    private CapsuleCollider _capsuleCollider;

    private void Awake()
    {
        _rigidbodyCharacterController = GetComponent<RigidbodyCharacterController>();
        _movementManager = GetComponent<MovementManager>();
        _groundedManager = GetComponent<GroundedManager>();
        _slidingManager = GetComponent<SlidingManager>();
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint? touchingBelowMaximumHeight = null;
        ContactPoint? touchingAboveMaximumHeight = null;

        foreach (var contact in collision.contacts)
        {
            isTouchingWallInFront = Vector3.Dot(contact.normal, -transform.forward) > wallDetectionAngleThreshold;

            if (isTouchingWallInFront && !_groundedManager.IsGrounded && IsMovingForward && !_slidingManager.IsSliding)
            {
                if (contact.point.y <= maximumHeightCollisionPoint.y)
                {
                    touchingBelowMaximumHeight = contact;
                }
                else
                {
                    touchingAboveMaximumHeight = contact;
                }
            }
        }

        if (touchingBelowMaximumHeight != null && touchingAboveMaximumHeight == null)
        {
            OnMantle?.Invoke();

            Mantle();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isTouchingWallInFront = false;
    }

    private void Mantle()
    {
        _rigidbody.MovePosition(transform.position + transform.forward + transform.up);
        // TODO: make a transition in the future and store the velocity to apply it

        _rigidbody.linearVelocity = transform.forward * _movementManager.topSpeed;
    }

    private void FixedUpdate()
    {
        RefreshMinimumHeightCollisionPoint();
    }

    private void RefreshMinimumHeightCollisionPoint()
    {
        maximumHeightCollisionPoint = _rigidbody.position + _capsuleCollider.center + Vector3.up * 0.1f;
    }
}
