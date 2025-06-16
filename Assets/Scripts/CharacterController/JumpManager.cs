using UnityEngine;
using UnityEngine.Events;

public class JumpManager : MonoBehaviour
{
    public float jumpHeight = 2f;

    public float jumpBufferTime = 0.15f;
    public float coyoteTime = 0.15f;

    public UnityEvent OnJump;

    private float _jumpBufferCounter = 0.15f;
    private float _coyoteTimeCounter = 0.15f;

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
        if (_rigidbodyCharacterController.currentInputPayload.JumpPressed)
        {
            ResetJumpBuffer();
        }

        UpdateCoyoteTimeCounter();
        UpdateJumpBufferCounter();

        if ((_coyoteTimeCounter > 0f || _groundedManager.IsGrounded) && _jumpBufferCounter > 0f)
        {
            ExecuteJump();
            OnJump?.Invoke();
        }
    }

    private void UpdateJumpBufferCounter()
    {
        if (_jumpBufferCounter > 0f)
        {
            _jumpBufferCounter -= Time.fixedDeltaTime;
        }
    }

    private void UpdateCoyoteTimeCounter()
    {
        if (_groundedManager.IsGrounded)
        {
            _coyoteTimeCounter = coyoteTime;
        }
        else
        {
            if (_coyoteTimeCounter > 0f)
            {
                _coyoteTimeCounter -= Time.fixedDeltaTime;
            }
        }
    }

    public void ResetJumpBuffer()
    {
        _jumpBufferCounter = jumpBufferTime;
    }

    public void ExecuteJump()
    {
        _rigidbody.linearVelocity = new Vector3()
        {
            x = _rigidbody.linearVelocity.x,
            y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y * _rigidbodyCharacterController.gravityScale),
            z = _rigidbody.linearVelocity.z
        };
    }
}
