using UnityEngine;
using UnityEngine.Events;

public class GroundJumpManager : MonoBehaviour
{
    public float jumpHeight = 2f;

    public float jumpBufferTime = 0.15f;
    public float coyoteTime = 0.15f;

    public UnityEvent OnJump;

    private float _jumpBufferCounter;
    private float _coyoteTimeCounter;

    private Rigidbody _rigidbody;
    private GroundedManager _groundedManager;
    private RigidbodyCharacterController _rigidbodyCharacterController;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _groundedManager = GetComponent<GroundedManager>();
        _rigidbodyCharacterController = GetComponent<RigidbodyCharacterController>();
    }

    private void FixedUpdate()
    {
        UpdateCoyoteTimeCounter();
        UpdateJumpBufferCounter();

        if ((_coyoteTimeCounter > 0f || _groundedManager.IsGrounded) && _jumpBufferCounter > 0f)
        {
            ExecuteJump();
            OnJump?.Invoke();
            CancelCoyoteTimeCounter();
            CancelJumpBufferCounter();
        }
    }

    private void CancelCoyoteTimeCounter()
    {
        _coyoteTimeCounter = 0f;
    }

    private void CancelJumpBufferCounter()
    {
        _jumpBufferCounter = 0f;
    }

    private void UpdateJumpBufferCounter()
    {
        if (_jumpBufferCounter > 0f)
        {
            _jumpBufferCounter -= Time.fixedDeltaTime;
        }
        else
        {
            if (_jumpBufferCounter < 0f)
            {
                _jumpBufferCounter = 0f;
            }
        }
    }

    private void UpdateCoyoteTimeCounter()
    {
        if (!_groundedManager.IsGrounded)
        {
            if (_coyoteTimeCounter > 0f)
            {
                _coyoteTimeCounter -= Time.fixedDeltaTime;
            }
            else
            {
                if (_coyoteTimeCounter < 0f)
                {
                    _coyoteTimeCounter = 0f;
                }
            }
        }
    }

    public void ResetJumpBufferCounter()
    {
        _jumpBufferCounter = jumpBufferTime;
    }

    public void ResetCoyoteTimeCounter()
    {
        _coyoteTimeCounter = coyoteTime;
    }

    public void ExecuteJump()
    {
        _rigidbody.linearVelocity = new Vector3()
        {
            x = _rigidbody.linearVelocity.x,
            y = Mathf.Sqrt(-2f * Physics.gravity.y * _rigidbodyCharacterController.gravityScale * jumpHeight),
            z = _rigidbody.linearVelocity.z
        };
    }
}
