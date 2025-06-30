using UnityEngine;

public class GravityModule : MonoBehaviour
{
    public float defaultGravityScale = 1.5f;

    private Rigidbody _rigidbody;

    private GroundCheckModule _groundCheckModule;
    private WallRunModule _wallRunModule;
    private SlidingModule _slidingModule;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _groundCheckModule = GetComponent<GroundCheckModule>();
        _wallRunModule = GetComponent<WallRunModule>();
        _slidingModule = GetComponent<SlidingModule>();
    }

    private void FixedUpdate()
    {
        var gravityScale = defaultGravityScale;

        if (_slidingModule.IsSliding)
        {
            gravityScale = _slidingModule.gravityScale;
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
