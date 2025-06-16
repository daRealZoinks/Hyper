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

    public float wallDetectionAngleThreshold = 0.9f;

    public float wallRunInitialImpulse = 5f;
    public float wallJumpHeight = 1.5f;
    public float wallJumpSideForce = 4f;
    public float wallJumpForwardForce = 1f;

    public float wallRunGravity = 0.75f; // Lower gravity while wallrunning

    public new Camera camera;

    public UnityEvent OnJump;
    public UnityEvent OnLanded;
    public UnityEvent OnStartedWallRunningRight;
    public UnityEvent OnStartedWallRunningLeft;

    public Vector2 MoveInput { private get; set; }

    private bool IsGrounded;


    private bool isTouchingWallOnRight;
    private bool isTouchingWallOnLeft;

    private bool isWallRunningOnRightWall;
    private bool isWallRunningOnLeftWall;

    private bool isWallRunning;

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

        CheckIfIsWallRunning();

        if (!IsGrounded)
        {
            ApplyCustomGravity(isWallRunning ? wallRunGravity : gravityScale);
        }

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
                    continue;
                }
            }

            var topCollisionPoint = _rigidbody.position + _collider.center + Vector3.up * (_collider.height / 2);
            var middleCollisionPoint = _rigidbody.position + _collider.center + Vector3.up * 0.1f;

            if (contact.point.y <= topCollisionPoint.y && contact.point.y >= middleCollisionPoint.y)
            {
                isTouchingWallOnRight = Vector3.Dot(contact.normal, -transform.right) > wallDetectionAngleThreshold;
                isTouchingWallOnLeft = Vector3.Dot(contact.normal, transform.right) > wallDetectionAngleThreshold;

                if (!IsGrounded)
                {
                    var force = Vector3.zero;

                    if (isTouchingWallOnRight)
                    {
                        OnStartedWallRunningRight?.Invoke();
                        force = Vector3.Cross(-wallNormal, Vector3.up) * wallRunInitialImpulse;

                        Debug.DrawLine(contact.point, contact.point - wallNormal, Color.red, 2f);
                    }

                    if (isTouchingWallOnLeft)
                    {
                        OnStartedWallRunningLeft?.Invoke();
                        force = Vector3.Cross(wallNormal, Vector3.up) * wallRunInitialImpulse;

                        Debug.DrawLine(contact.point, contact.point + wallNormal, Color.red, 2f);
                    }

                    Debug.DrawLine(contact.point, contact.point + Vector3.up, Color.green, 2f);

                    Debug.DrawLine(contact.point, contact.point + force, Color.blue, 2f);

                    _rigidbody.AddForce(force, ForceMode.VelocityChange);
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        isWallRunningOnRightWall = false;
        isWallRunningOnLeftWall = false;

        foreach (var contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                IsGrounded = true;
                wallNormal = Vector3.zero;
                continue;
            }

            var topCollisionPoint = _rigidbody.position + _collider.center + Vector3.up * (_collider.height / 2);
            var middleCollisionPoint = _rigidbody.position + _collider.center + Vector3.up * 0.1f;

            if (contact.point.y <= topCollisionPoint.y && contact.point.y >= middleCollisionPoint.y)
            {
                isTouchingWallOnRight = Vector3.Dot(contact.normal, -transform.right) > wallDetectionAngleThreshold;
                isTouchingWallOnLeft = Vector3.Dot(contact.normal, transform.right) > wallDetectionAngleThreshold;

                wallNormal = contact.normal;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        IsGrounded = false;

        isTouchingWallOnRight = false;
        isTouchingWallOnLeft = false;

        wallNormal = Vector3.zero;
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

    private void ApplyCustomGravity(float gravityScale)
    {
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

        if (!IsGrounded && !isWallRunning)
        {
            finalForce *= (inputDirection != Vector3.zero) ? airControl : airBreak;
        }

        _rigidbody.AddForce(finalForce, ForceMode.Acceleration);
    }

    private void CheckIfIsWallRunning()
    {
        isWallRunningOnRightWall = isTouchingWallOnRight && !IsGrounded;
        isWallRunningOnLeftWall = isTouchingWallOnLeft && !IsGrounded;

        isWallRunning = isWallRunningOnRightWall || isWallRunningOnLeftWall;
    }

    public void Jump()
    {
        if (IsGrounded)
        {
            ExecuteJump();
            OnJump.Invoke();
        }
        // else if (IsWallRunning)
        // {
        //     ExecuteWallJump();
        // }
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
        // Use the stored wall normal for the jump
        var sideForce = wallNormal * wallJumpSideForce;
        var jumpForce = Vector3.up * Mathf.Sqrt(-2 * Physics.gravity.y * gravityScale * wallJumpHeight);
        var forwardForce = transform.forward * wallJumpForwardForce;

        var finalForce = sideForce + jumpForce + forwardForce;

        _rigidbody.linearVelocity = new Vector3()
        {
            x = _rigidbody.linearVelocity.x,
            z = _rigidbody.linearVelocity.z
        };

        _rigidbody.AddForce(finalForce, ForceMode.VelocityChange);

        isWallRunningOnRightWall = false;
        isWallRunningOnLeftWall = false;
        wallNormal = Vector3.zero;
    }
}
