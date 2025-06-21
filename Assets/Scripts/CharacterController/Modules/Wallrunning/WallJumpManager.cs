using UnityEngine;
using UnityEngine.Events;

public class WallJumpManager : MonoBehaviour
{
    public float wallJumpHeight = 1.5f;
    public float wallJumpSideForce = 4f;
    public float wallJumpForwardForce = 1f;

    public float jumpBufferTime = 0.15f;

    public UnityEvent OnWallJump;

    private float _jumpBufferCounter;

    private WallRunManager _wallRunManager;
    private RigidbodyCharacterController _rigidbodyCharacterController;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _wallRunManager = GetComponent<WallRunManager>();
        _rigidbodyCharacterController = GetComponent<RigidbodyCharacterController>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        UpdateJumpBufferCounter();

        if (_wallRunManager.IsWallRunning && _jumpBufferCounter > 0f)
        {
            ExecuteWallJump();
            OnWallJump?.Invoke();

            if (_wallRunManager.IsWallRunningOnRightWall)
            {
                _wallRunManager.HasWallRunOnRight = true;
                _wallRunManager.HasWallRunOnLeft = false;
            }

            if (_wallRunManager.IsWallRunningOnLeftWall)
            {
                _wallRunManager.HasWallRunOnLeft = true;
                _wallRunManager.HasWallRunOnRight = false;
            }

            _jumpBufferCounter = 0f;
        }
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

    public void ResetJumpBuffer()
    {
        _jumpBufferCounter = jumpBufferTime;
    }

    private void ExecuteWallJump()
    {
        var sideForce = _wallRunManager.WallNormal * wallJumpSideForce;
        var jumpForce = Vector3.up * Mathf.Sqrt(-2 * Physics.gravity.y * _rigidbodyCharacterController.gravityScale * wallJumpHeight);
        var forwardForce = transform.forward * wallJumpForwardForce;

        var finalForce = sideForce + jumpForce + forwardForce;

        _rigidbody.linearVelocity = new Vector3()
        {
            x = _rigidbody.linearVelocity.x,
            z = _rigidbody.linearVelocity.z
        };

        _rigidbody.AddForce(finalForce, ForceMode.VelocityChange);
    }
}
