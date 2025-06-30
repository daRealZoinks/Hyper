using UnityEngine;

public class JumpManager : MonoBehaviour
{
    public float jumpBufferTime = 0.15f;

    private float _jumpBufferCounter;

    private GroundCheckModule _groundCheckModule;
    private GroundJumpModule _groundJumpModule;
    private WallJumpModule _wallJumpModule;
    private RigidbodyCharacterController _rigidbodyCharacterController;

    private void Awake()
    {
        _groundCheckModule = GetComponent<GroundCheckModule>();
        _groundJumpModule = GetComponent<GroundJumpModule>();
        _wallJumpModule = GetComponent<WallJumpModule>();
        _rigidbodyCharacterController = GetComponent<RigidbodyCharacterController>();
    }

    private void FixedUpdate()
    {
        UpdateJumpBufferCounter();

        if (_rigidbodyCharacterController.currentInputPayload.JumpPressed)
        {
            ResetJumpBufferCounter();
        }

        if (_jumpBufferCounter > 0f)
        {
            _groundJumpModule.Jump();

            if (!_groundCheckModule.IsGrounded)
            {
                _wallJumpModule.WallJump();
            }
        }
    }

    private void ResetJumpBufferCounter()
    {
        _jumpBufferCounter = jumpBufferTime;
    }

    public void CancelJumpBufferCounter()
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
}
