using UnityEngine;

public class JumpManager : MonoBehaviour
{
    private GroundedManager _groundedManager;
    private GroundJumpManager _groundJumpManager;
    private WallJumpManager _wallJumpManager;
    private RigidbodyCharacterController _rigidbodyCharacterController;

    private void Awake()
    {
        _groundedManager = GetComponent<GroundedManager>();
        _groundJumpManager = GetComponent<GroundJumpManager>();
        _wallJumpManager = GetComponent<WallJumpManager>();
        _rigidbodyCharacterController = GetComponent<RigidbodyCharacterController>();
    }

    private void FixedUpdate()
    {
        if (_rigidbodyCharacterController.currentInputPayload.JumpPressed)
        {
            _groundJumpManager.ResetJumpBufferCounter();
            if (!_groundedManager.IsGrounded)
            {
                _wallJumpManager.ResetJumpBuffer();
            }
        }
    }
}
