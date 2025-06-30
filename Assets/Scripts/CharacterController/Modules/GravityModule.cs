using UnityEngine;

public class GravityModule : MonoBehaviour
{
    public float gravityScale = 1.5f;

    private Rigidbody _rigidbody;

    private GroundedManager _groundedManager;
    private WallRunManager _wallRunManager;
    private SlidingManager _slidingManager;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _groundedManager = GetComponent<GroundedManager>();
        _wallRunManager = GetComponent<WallRunManager>();
        _slidingManager = GetComponent<SlidingManager>();
    }

    private void FixedUpdate()
    {
        if (!_groundedManager.IsGrounded && !_wallRunManager.IsWallRunning && !_slidingManager.IsSliding)
        {
            ApplyCustomGravity(gravityScale);
        }
    }

    private void ApplyCustomGravity(float gravityScale)
    {
        _rigidbody.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);
    }
}
