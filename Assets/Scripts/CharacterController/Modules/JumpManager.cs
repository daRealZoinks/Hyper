using UnityEngine;

public class JumpManager : MonoBehaviour
{
    private GroundJumpManager _groundJumpManager;
    private RigidbodyCharacterController _rigidbodyCharacterController;

    private void Awake()
    {
        _groundJumpManager = GetComponent<GroundJumpManager>();
        _rigidbodyCharacterController = GetComponent<RigidbodyCharacterController>();
    }

    private void FixedUpdate()
    {
        if (_rigidbodyCharacterController.currentInputPayload.JumpPressed)
        {
            _groundJumpManager.ResetJumpBuffer();
        }
    }
}
