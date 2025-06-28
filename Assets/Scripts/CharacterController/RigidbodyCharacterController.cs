using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class RigidbodyCharacterController : MonoBehaviour
{
    public float gravityScale = 1.5f;

    public Vector2 MoveInput { private get; set; }
    public bool JumpPressed { private get; set; }
    public bool Sliding { private get; set; }

    public struct InputPayload
    {
        public Vector2 MoveInput;
        public bool JumpPressed;
        public bool Sliding;
    }

    public InputPayload currentInputPayload;

    private Rigidbody _rigidbody;
    private Camera _camera;

    private GroundedManager _groundedManager;
    private WallRunManager _wallRunManager;
    private SlidingManager _slidingManager;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _camera = GetComponent<PlayerInput>().camera;

        _groundedManager = GetComponent<GroundedManager>();
        _wallRunManager = GetComponent<WallRunManager>();
        _slidingManager = GetComponent<SlidingManager>();
    }

    private void FixedUpdate()
    {
        UpdateRotationBasedOnCamera();

        if (!_groundedManager.IsGrounded && !_wallRunManager.IsWallRunning && !_slidingManager.IsSliding)
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
            JumpPressed = JumpPressed,
            Sliding = Sliding,
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
        _rigidbody.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);
    }
}
