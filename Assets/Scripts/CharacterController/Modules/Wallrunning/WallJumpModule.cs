using UnityEngine;
using UnityEngine.Events;

public class WallJumpModule : MonoBehaviour
{
    [SerializeField]
    private float wallJumpHeight = 1.5f;
    [SerializeField]
    private float wallJumpSideForce = 4f;
    [SerializeField]
    private float wallJumpForwardForce = 5f;
    [SerializeField]
    private float sameWallJumpCooldown = 2.5f;

    public UnityEvent OnRightWallJump;
    public UnityEvent OnLeftWallJump;

    private GameObject lastWallJumped;

    private float _sameWallJumpCooldownCounter;

    private GroundCheckModule _groundCheckModule;
    private WallRunModule _wallRunModule;
    private GravityModule _gravityModule;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _groundCheckModule = GetComponent<GroundCheckModule>();
        _wallRunModule = GetComponent<WallRunModule>();
        _gravityModule = GetComponent<GravityModule>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        UpdateSameWallJumpCooldownCounter();

        if (_groundCheckModule.IsGrounded)
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
        var sideForce = _wallRunModule.WallContactPoint.normal * wallJumpSideForce;
        var jumpForce = Vector3.up * Mathf.Sqrt(-2 * Physics.gravity.y * _gravityModule.defaultGravityScale * wallJumpHeight);
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
