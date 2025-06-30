using UnityEngine;

public class GravityModule : MonoBehaviour
{
    public float defaultGravityScale = 1.5f;

    private Rigidbody _rigidbody;

    private GroundCheckModule _groundedManager;
    private WallRunManager _wallRunManager;
    private SlidingManager _slidingManager;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _groundedManager = GetComponent<GroundCheckModule>();
        _wallRunManager = GetComponent<WallRunManager>();
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
            if (_wallRunManager.IsWallRunning)
            {
                var wallRunGravity = _rigidbody.linearVelocity.y >= 0 ? _wallRunManager.wallRunAscendingGravity : _wallRunManager.wallRunDescendingGravity;

                gravityScale = wallRunGravity;
            }
            else
            {
                if (_groundedManager.IsGrounded)
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
