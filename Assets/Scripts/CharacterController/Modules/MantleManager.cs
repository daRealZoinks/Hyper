using System.Collections;
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

    private Vector3 _storedVelocity;
    private bool _isMantling;

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
            isTouchingWallInFront = Vector3.Dot(contact.normal, -transform.forward) > wallDetectionAngleThreshold && contact.normal.y == 0;

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
        if (_isMantling) return;

        var start = transform.position;
        var end = start + transform.forward + transform.up;

        _storedVelocity = transform.forward * _movementManager.topSpeed;

        var mantleVelocity = _movementManager.topSpeed;
        StartCoroutine(MantleTransition(start, end, mantleVelocity));
    }

    private IEnumerator MantleTransition(Vector3 start, Vector3 end, float velocity)
    {
        _isMantling = true;
        var distance = Vector3.Distance(start, end);
        var traveled = 0f;

        while (traveled < distance)
        {
            var step = velocity * Time.fixedDeltaTime;
            traveled += step;
            var t = Mathf.Clamp01(traveled / distance);
            transform.position = Vector3.Lerp(start, end, t);
            yield return new WaitForFixedUpdate();
        }

        transform.position = end;

        _rigidbody.linearVelocity = _storedVelocity;

        _isMantling = false;
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
