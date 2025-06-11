using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class RigidbodyCharacterController : MonoBehaviour
{
    public float acceleration = 60f;
    public float topSpeed = 8f;
    public float deceleration = 120f;

    public float airControl = 1f;
    public float airBreak = 1f;

    public float jumpHeight = 2f;
    public float gravityScale = 1.5f;

    public float wallRunInitialImpulse = 5f;
    public float wallCheckDistance = 0.75f;
    public float wallJumpHeight = 1.5f;
    public float wallJumpSideForce = 4f;
    public float wallJumpForwardForce = 1f;

    public new Camera camera;

    public UnityEvent OnJump;
    public UnityEvent OnLanded;
    public UnityEvent OnStartedWallRunningRight;
    public UnityEvent OnStartedWallRunningLeft;

    public Vector2 MoveInput { private get; set; }

    public bool IsGrounded { get; private set; }
    public bool IsWallRight { get; private set; }
    public bool IsWallLeft { get; private set; }

    private Vector3 wallNormal;

    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
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
                Debug.DrawLine(contact.point, contact.point + contact.normal, Color.green, 1f);

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
                IsWallRight = false;
                IsWallLeft = false;
                break;
            }

            if (IsGrounded) return;

            const float wallThreshold = 0.9f; // temporary

            var topCollisionPoint = transform.position + _collider.center + Vector3.up * (_collider.height / 2 - _collider.radius);
            var middleCollisionPoint = transform.position + _collider.center + Vector3.up * 0.1f;

            if (contact.point.y > topCollisionPoint.y || contact.point.y < middleCollisionPoint.y)
            {
                continue;
            }

            CheckWallRight(contact, wallThreshold);
            CheckWallLeft(contact, wallThreshold);
        }
    }

    private void CheckWallLeft(ContactPoint contact, float wallThreshold)
    {
        var wasWallLeft = IsWallLeft;
        IsWallLeft = Vector3.Dot(contact.normal, transform.right) > wallThreshold;

        if (IsWallLeft)
        {
            // currently touching the wall on the left side
        }

        if (!wasWallLeft && IsWallLeft)
        {
            var forwardForce = Vector3.Cross(contact.normal, transform.up) * wallRunInitialImpulse;
            _rigidbody.AddForce(forwardForce, ForceMode.VelocityChange);

            OnStartedWallRunningLeft?.Invoke();
        }
    }

    private void CheckWallRight(ContactPoint contact, float wallThreshold)
    {
        var wasWallRight = IsWallRight;
        IsWallRight = Vector3.Dot(contact.normal, -transform.right) > wallThreshold;

        if (IsWallRight)
        {
            // currently touching the wall on the right side
        }

        if (!wasWallRight && IsWallRight)
        {
            var forwardForce = Vector3.Cross(-contact.normal, transform.up) * wallRunInitialImpulse;
            _rigidbody.AddForce(forwardForce, ForceMode.VelocityChange);

            OnStartedWallRunningRight?.Invoke();
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

        var finalForce = inputDirection - horizontalRigidbodyVelocity / topSpeed;

        finalForce *= (inputDirection != Vector3.zero) ? acceleration : deceleration;

        if (!IsGrounded)
        {
            finalForce *= (inputDirection != Vector3.zero) ? airControl : airBreak;
        }

        _rigidbody.AddForce(finalForce, ForceMode.Acceleration);
    }

    public void Jump()
    {
        if (IsGrounded)
        {
            ExecuteJump();

            OnJump.Invoke();
        }
        else
        {
            if (IsWallRight || IsWallLeft)
            {
                ExecuteWallJump();
            }
        }
    }

    private void ExecuteJump()
    {
        _rigidbody.linearVelocity = new Vector3()
        {
            x = _rigidbody.linearVelocity.x,
            y = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y * gravityScale),
            z = _rigidbody.linearVelocity.z
        };

        IsGrounded = false;
    }

    private void ExecuteWallJump()
    {
        var sideForce = Vector3.zero;
        if (IsWallRight) sideForce = _rightHitInfo.normal * wallJumpSideForce;
        if (IsWallLeft) sideForce = _leftHitInfo.normal * wallJumpSideForce;

        var jumpForce = Vector3.up * Mathf.Sqrt(-2 * Physics.gravity.y * gravityScale * wallJumpHeight);

        var forwardForce = transform.forward * wallJumpForwardForce;

        var finalForce = sideForce + jumpForce + forwardForce;

        _rigidbody.linearVelocity = new Vector3()
        {
            x = _rigidbody.linearVelocity.x,
            z = _rigidbody.linearVelocity.z
        };

        _rigidbody.AddForce(finalForce, ForceMode.VelocityChange);

        IsWallRight = false;
        IsWallLeft = false;
    }
}
