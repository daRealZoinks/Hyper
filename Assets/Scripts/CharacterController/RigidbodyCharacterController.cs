using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class RigidbodyCharacterController : MonoBehaviour
{
    public float gravityScale = 1.5f;

    public Vector2 MoveInput { private get; set; }
    public bool JumpPressed { private get; set; }

    public struct InputPayload
    {
        public Vector2 MoveInput;
        public bool JumpPressed;
    }

    public InputPayload currentInputPayload;

    private Rigidbody _rigidbody;
    private Camera _camera;

    private GroundedManager _groundedManager;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _camera = GetComponent<PlayerInput>().camera;

        _groundedManager = GetComponent<GroundedManager>();
    }

    private void FixedUpdate()
    {
        UpdateRotationBasedOnCamera();

        if (!_groundedManager.IsGrounded)
        {
            ApplyCustomGravity(gravityScale);
        }

        UpdateCurrentInputPayload();
    }

    private void UpdateCurrentInputPayload()
    {
        currentInputPayload = new InputPayload
        {
            MoveInput = MoveInput,
            JumpPressed = JumpPressed
        };

        if (JumpPressed)
        {
            JumpPressed = false;
        }
    }

    private void UpdateRotationBasedOnCamera()
    {
        var cameraRotation = _camera.transform.rotation.eulerAngles;
        var cameraYRotation = Quaternion.Euler(0, cameraRotation.y, 0);
        _rigidbody.rotation = cameraYRotation;
    }

    private void ApplyCustomGravity(float gravityScale)
    {
        var gravity = Physics.gravity * (gravityScale - 1);
        _rigidbody.AddForce(gravity, ForceMode.Acceleration);
    }
}
