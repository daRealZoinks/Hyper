using UnityEngine;
using UnityEngine.InputSystem;

public class RigidbodyCharacterController : MonoBehaviour
{
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

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _camera = GetComponent<PlayerInput>().camera;
    }

    private void FixedUpdate()
    {
        UpdateRotationBasedOnCamera();

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
}
