using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class RigidbodyCharacterController : MonoBehaviour
{
    public float gravityScale = 1.5f;

    public bool IsGrounded => _groundedManager.IsGrounded;

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
    private MovementManager _movementManager;
    private JumpManager _jumpManager;









    public float wallRunInitialImpulse = 5f;
    public float wallCheckDistance = 0.75f;
    public float wallJumpHeight = 1.5f;
    public float wallJumpSideForce = 4f;
    public float wallJumpForwardForce = 1f;



    public bool IsWallRight { get; private set; }
    public bool IsWallLeft { get; private set; }

    public bool HasWallRunRight { get; private set; }
    public bool HasWallRunLeft { get; private set; }

    private CapsuleCollider _collider;

    private RaycastHit _rightHitInfo;
    private RaycastHit _leftHitInfo;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _camera = GetComponent<PlayerInput>().camera;

        _groundedManager = GetComponent<GroundedManager>();
        _movementManager = GetComponent<MovementManager>();
        _jumpManager = GetComponent<JumpManager>();

        _collider = GetComponent<CapsuleCollider>();
    }

    private void FixedUpdate()
    {
        UpdateRotationBasedOnCamera();

        if (!IsGrounded)
        {
            ApplyCustomGravity(gravityScale);
        }

        currentInputPayload = new InputPayload
        {
            MoveInput = MoveInput,
            JumpPressed = JumpPressed
        };

        if (JumpPressed)
        {
            JumpPressed = false; // Reset jump pressed state
        }





        _movementManager.Move(MoveInput);
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
