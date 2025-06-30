using UnityEngine;
using UnityEngine.Events;

public class WallJumpManager : MonoBehaviour
{
    public float wallJumpHeight = 1.5f;
    public float wallJumpSideForce = 4f;
    public float wallJumpForwardForce = 5f;

    public float jumpBufferTime = 0.15f;
    public float sameWallJumpCooldown = 2.5f;

    public UnityEvent OnWallJump;

    private GameObject lastWallJumped;

    private float _jumpBufferCounter;
    private float _sameWallJumpCooldownCounter;

    private GroundCheckModule _groundedManager;
    private WallRunManager _wallRunManager;
    private GravityModule _gravityModule;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _groundedManager = GetComponent<GroundCheckModule>();
        _wallRunManager = GetComponent<WallRunManager>();
        _gravityModule = GetComponent<GravityModule>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        UpdateJumpBufferCounter();
        UpdateSameWallJumpCooldownCounter();

        if (_groundedManager.IsGrounded)
        {
            lastWallJumped = null;
            _sameWallJumpCooldownCounter = 0f;
        }

        if (_wallRunManager.IsWallRunning && _jumpBufferCounter > 0f)
        {
            var currentWall = _wallRunManager.WallRunningWall;

            if (!currentWall || currentWall != lastWallJumped || _sameWallJumpCooldownCounter <= 0f)
            {
                lastWallJumped = currentWall;
                _sameWallJumpCooldownCounter = sameWallJumpCooldown;
                WallJump();
            }
        }
    }

    private void WallJump()
    {
        ExecuteWallJump();
        OnWallJump?.Invoke();
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
    private void UpdateSameWallJumpCooldownCounter()
    {
        if (_sameWallJumpCooldownCounter > 0f)
        {
            _sameWallJumpCooldownCounter -= Time.fixedDeltaTime;
        }
        else
        {
            if (_sameWallJumpCooldownCounter < 0f)
            {
                _sameWallJumpCooldownCounter = 0f;
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
        var jumpForce = Vector3.up * Mathf.Sqrt(-2 * Physics.gravity.y * _gravityModule.defaultGravityScale * wallJumpHeight);
        var forwardForce = transform.forward * wallJumpForwardForce;

        var finalForce = sideForce + jumpForce + forwardForce;

        _rigidbody.linearVelocity = new Vector3()
        {
            x = _rigidbody.linearVelocity.x,
            y = 0,
            z = _rigidbody.linearVelocity.z
        };

        _rigidbody.AddForce(finalForce, ForceMode.VelocityChange);
    }
}
