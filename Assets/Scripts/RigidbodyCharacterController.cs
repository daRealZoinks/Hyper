using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyCharacterController : MonoBehaviour
{
    public float acceleration = 60f;
    public float topSpeed = 8f;
    public float deceleration = 60f;

    public float airControl = 1f;
    public float airBreak = 1f;

    public float jumpHeight = 2f;
    public float gravityScale = 1.5f;

    public new Camera camera;

    public UnityEvent OnJump;
    public UnityEvent OnLanded;

    public Vector2 MoveInput { private get; set; }

    public bool IsGrounded { get; private set; }

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Move(MoveInput);
        ApplyGravity();
        UpdateRotationBasedOnCamera();
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                if (!IsGrounded)
                {
                    IsGrounded = true;
                    OnLanded?.Invoke();
                }
                break;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                IsGrounded = true;
                break;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        IsGrounded = false;
    }

    private void UpdateRotationBasedOnCamera()
    {
        if (camera == null)
        {
            return;
        }

        var cameraRotation = camera.transform.rotation.eulerAngles;
        var cameraYRotation = Quaternion.Euler(0, cameraRotation.y, 0);
        _rigidbody.rotation = cameraYRotation;
    }

    private void ApplyGravity()
    {
        if (IsGrounded)
        {
            return;
        }
        var gravity = Physics.gravity * (gravityScale - 1);
        _rigidbody.AddForce(gravity, ForceMode.Acceleration);
    }

    private void Move(Vector2 moveInput)
    {
        var inputDirection = transform.right * moveInput.x + transform.forward * moveInput.y;

        var horizontalRigidbodyVelocity = new Vector3
        {
            x = _rigidbody.linearVelocity.x,
            z = _rigidbody.linearVelocity.z
        };

        var horizontalClampedRigidbodyVelocity = horizontalRigidbodyVelocity.normalized * Mathf.Clamp01(horizontalRigidbodyVelocity.magnitude / topSpeed);

        Vector3 finalForce = inputDirection - horizontalClampedRigidbodyVelocity;

        finalForce *= (inputDirection != Vector3.zero) ? acceleration : deceleration;

        if (!IsGrounded)
        {
            finalForce *= (inputDirection != Vector3.zero) ? airControl : airBreak;
        }

        _rigidbody.AddForce(finalForce, ForceMode.Acceleration);
    }

    public void Jump()
    {
        if (IsGrounded == false)
        {
            return;
        }

        OnJump.Invoke();

        ExecuteJump();
    }

    private void ExecuteJump()
    {
        var jumpForce = Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y * gravityScale);

        _rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);

        IsGrounded = false;
    }
}
