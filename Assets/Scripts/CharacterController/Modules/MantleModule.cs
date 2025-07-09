using UnityEngine;
using UnityEngine.Events;

public class MantleModule : MonoBehaviour
{
    [SerializeField]
    private float wallDetectionAngleThreshold = 0.9f;
    [SerializeField]
    private float mantleDuration = 0.2f;

    public UnityEvent OnMantle;

    public bool IsMantling { get; private set; }

    public bool IsMovingForward => _rigidbodyCharacterController.CurrentInputPayload.MoveInput.y > 0;

    private bool _isTouchingWallInFront;

    private Vector3 _mantleStart;
    private Vector3 _mantleEnd;

    private float _mantleElapsedTime;

    private RigidbodyCharacterController _rigidbodyCharacterController;
    private GroundCheckModule _groundCheckModule;
    private SlidingModule _slidingModule;
    private Rigidbody _rigidbody;
    private CapsuleCollider _capsuleCollider;

    private void Awake()
    {
        _rigidbodyCharacterController = GetComponent<RigidbodyCharacterController>();
        _groundCheckModule = GetComponent<GroundCheckModule>();
        _slidingModule = GetComponent<SlidingModule>();
        _rigidbody = GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint? touchingBelowMaximumHeight = null;
        ContactPoint? touchingAboveMaximumHeight = null;

        foreach (var contact in collision.contacts)
        {
            _isTouchingWallInFront = Vector3.Dot(contact.normal, -transform.forward) > wallDetectionAngleThreshold && contact.normal.y == 0;

            if (_isTouchingWallInFront && !_groundCheckModule.IsGrounded && IsMovingForward && !_slidingModule.IsSliding)
            {
                var maximumHeightCollisionPoint = _rigidbody.position + _capsuleCollider.center + Vector3.up * 0.1f;

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

            Mantle(touchingBelowMaximumHeight.Value);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        _isTouchingWallInFront = false;
    }

    private void Mantle(ContactPoint touchingBelowMaximumHeight)
    {
        if (IsMantling) return;

        var capsuleColliderCenterPosition = transform.position;
        var mantleForwardOffset = transform.forward * _capsuleCollider.radius;
        var mantleVerticalOffset = transform.up * (touchingBelowMaximumHeight.point.y - capsuleColliderCenterPosition.y);

        _mantleStart = capsuleColliderCenterPosition;
        _mantleEnd = _mantleStart + mantleForwardOffset + mantleVerticalOffset;
        _mantleElapsedTime = 0f;

        IsMantling = true;
    }

    private void Update()
    {
        if (IsMantling)
        {
            _mantleElapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(_mantleElapsedTime / mantleDuration);
            transform.position = Vector3.Lerp(_mantleStart, _mantleEnd, t);

            if (_mantleElapsedTime >= mantleDuration)
            {
                transform.position = _mantleEnd;
                _rigidbody.linearVelocity = Vector3.zero;
                IsMantling = false;
            }
        }
    }
}
