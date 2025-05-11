using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyCharacterController : MonoBehaviour
{
    public float acceleration = 5f;
    public float topSpeed = 5f;
    public float deceleration = 5f;

    public float jumpHeight = 3f;
    public float gravityScale = 2f;

    public new Camera camera;

    public UnityEvent OnJump;
    public UnityEvent OnLanded;

    public Vector2 MoveInput { private get; set; }

    public bool Grounded { get; private set; }

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (camera == null)
        {
            return;
        }

        // snap y rotation to camera y rotation

        var cameraRotation = camera.transform.rotation.eulerAngles;
        var cameraYRotation = Quaternion.Euler(0, cameraRotation.y, 0);
        transform.rotation = cameraYRotation;
    }

    private void FixedUpdate()
    {
        Move(MoveInput);
        ApplyGravity();
    }

    private void ApplyGravity()
    {
        if (Grounded)
        {
            return;
        }
        var gravity = Physics.gravity * (gravityScale - 1);
        _rigidbody.AddForce(gravity, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision collision)
    {
        List<ContactPoint> _contacts = new();
        int contactCount = collision.GetContacts(_contacts);
        for (int i = 0; i < contactCount; i++)
        {
            if (_contacts[i].normal.y > 0.5f)
            {
                Grounded = true;

                OnLanded.Invoke();

                return;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Grounded = false;
    }

    private void Move(Vector2 moveInput)
    {
        var horizontalVelocity = new Vector3
        {
            x = _rigidbody.linearVelocity.x,
            z = _rigidbody.linearVelocity.z
        };

        var horizontalClampedVelocity = horizontalVelocity.normalized * (horizontalVelocity.magnitude / topSpeed);

        var horizontalInput = transform.right * moveInput.x;
        var verticalInput = transform.forward * moveInput.y;
        var inputDirection = (horizontalInput + verticalInput).normalized;

        Vector3 finalForce;

        if (inputDirection != Vector3.zero)
        {
            finalForce = (inputDirection - horizontalClampedVelocity) * acceleration;
        }
        else
        {
            finalForce = -horizontalClampedVelocity * deceleration;
        }

        _rigidbody.AddForce(finalForce, ForceMode.Acceleration);
    }

    public void Jump()
    {
        if (Grounded == false)
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

        Grounded = false;
    }
}
