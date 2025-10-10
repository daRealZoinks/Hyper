using UnityEngine;

public class OldRigidbodyCharacterController : MonoBehaviour
{
    [SerializeField]
    private new Camera camera;

    public Vector2 MoveInput { private get; set; }
    public bool JumpPressed { private get; set; }
    public bool Sliding { private get; set; }

    public struct InputPayload
    {
        public Vector2 MoveInput;
        public bool JumpPressed;
        public bool Sliding;
    }

    public InputPayload CurrentInputPayload;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        UpdateRotationBasedOnCamera();

        UpdateCurrentInputPayload();
    }

    private void UpdateCurrentInputPayload()
    {
        CurrentInputPayload = new InputPayload
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
        var cameraRotation = camera.transform.rotation.eulerAngles;
        var cameraYRotation = Quaternion.Euler(0, cameraRotation.y, 0);
        _rigidbody.rotation = cameraYRotation;
    }
}
