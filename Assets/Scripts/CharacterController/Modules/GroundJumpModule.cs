using UnityEngine;
using UnityEngine.Events;

public class GroundJumpModule : MonoBehaviour
{
    public float jumpHeight = 2f;

    public float coyoteTime = 0.15f;

    public UnityEvent OnJump;

    private float _coyoteTimeCounter;

    private Rigidbody _rigidbody;
    private GroundCheckModule _groundCheckModule;
    private GravityModule _gravityModule;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _groundCheckModule = GetComponent<GroundCheckModule>();
        _gravityModule = GetComponent<GravityModule>();
    }

    private void FixedUpdate()
    {
        UpdateCoyoteTimeCounter();
    }

    public void Jump()
    {
        if (_coyoteTimeCounter > 0f || _groundCheckModule.IsGrounded)
        {
            ExecuteJump();
            OnJump?.Invoke();
            CancelCoyoteTimeCounter();
        }
    }

    private void CancelCoyoteTimeCounter()
    {
        _coyoteTimeCounter = 0f;
    }

    private void UpdateCoyoteTimeCounter()
    {
        if (!_groundCheckModule.IsGrounded)
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

    public void ResetCoyoteTimeCounter()
    {
        _coyoteTimeCounter = coyoteTime;
    }

    public void ExecuteJump()
    {
        var jumpForce = Vector3.up * Mathf.Sqrt(-2f * Physics.gravity.y * _gravityModule.defaultGravityScale * jumpHeight);

        if (_rigidbody.linearVelocity.y < 0)
        {
            _rigidbody.linearVelocity = new Vector3()
            {
                x = _rigidbody.linearVelocity.x,
                y = 0,
                z = _rigidbody.linearVelocity.z
            };
        }

        _rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);
    }
}
