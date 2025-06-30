using UnityEngine;
using UnityEngine.Events;

public class WallJumpModule : MonoBehaviour
{
    public float wallJumpHeight = 1.5f;
    public float wallJumpSideForce = 4f;
    public float wallJumpForwardForce = 5f;

    public float sameWallJumpCooldown = 2.5f;

    public UnityEvent OnRightWallJump;
    public UnityEvent OnLeftWallJump;

    private GameObject lastWallJumped;

    private float _sameWallJumpCooldownCounter;

    private GroundCheckModule _groundedManager;
    private WallRunModule _wallRunModule;
    private GravityModule _gravityModule;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _groundedManager = GetComponent<GroundCheckModule>();
        _wallRunModule = GetComponent<WallRunModule>();
        _gravityModule = GetComponent<GravityModule>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        UpdateSameWallJumpCooldownCounter();

        if (_groundedManager.IsGrounded)
        {
            lastWallJumped = null;
            CancelSameWallJumpCooldownCounter();
        }
    }

    private void ResetSameWallJumpCooldownCounter()
    {
        _sameWallJumpCooldownCounter = sameWallJumpCooldown;
    }

    private void CancelSameWallJumpCooldownCounter()
    {
        _sameWallJumpCooldownCounter = 0f;
    }

    public void WallJump()
    {
        var currentWall = _wallRunModule.WallRunningWall;

        if ((!currentWall || currentWall != lastWallJumped || _sameWallJumpCooldownCounter <= 0f) && _wallRunModule.IsWallRunning)
        {
            lastWallJumped = currentWall;
            ResetSameWallJumpCooldownCounter();
            ExecuteWallJump();

            if (_wallRunModule.IsWallRunningOnRightWall)
            {
                OnRightWallJump?.Invoke();
            }

            if (_wallRunModule.IsWallRunningOnLeftWall)
            {
                OnLeftWallJump?.Invoke();
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

    private void ExecuteWallJump()
    {
        var sideForce = _wallRunModule.WallNormal * wallJumpSideForce;
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
