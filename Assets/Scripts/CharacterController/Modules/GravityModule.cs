using UnityEngine;

public class GravityModule : MonoBehaviour
{
    public float defaultGravityScale = 1.5f;

    private Rigidbody _rigidbody;

    private GroundCheckModule _groundCheckModule;
    private WallRunModule _wallRunModule;
    private SlidingManager _slidingManager;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _groundCheckModule = GetComponent<GroundCheckModule>();
        _wallRunModule = GetComponent<WallRunModule>();
        _slidingManager = GetComponent<SlidingManager>();
    }

    private void FixedUpdate()
    {
        var gravityScale = defaultGravityScale;

        if (_slidingManager.IsSliding)
        {
            gravityScale = _slidingManager.gravityScale;
        }
        else
        {
            if (_wallRunModule.IsWallRunning)
            {
                var wallRunGravity = _rigidbody.linearVelocity.y >= 0 ? _wallRunModule.wallRunAscendingGravity : _wallRunModule.wallRunDescendingGravity;

                gravityScale = wallRunGravity;
            }
            else
            {
                if (_groundCheckModule.IsGrounded)
                {
                    return;
                }
            }
        }

        ApplyCustomGravity(gravityScale);
    }

    private void ApplyCustomGravity(float gravityScale)
    {
        _rigidbody.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);
    }
}
