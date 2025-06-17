using UnityEngine;
using UnityEngine.Events;
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









    public float wallDetectionAngleThreshold = 0.9f;

    public float wallRunInitialImpulse = 5f;
    public float wallJumpHeight = 1.5f;
    public float wallJumpSideForce = 4f;
    public float wallJumpForwardForce = 1f;

    public float wallRunGravity = 0.75f; // Lower gravity while wallrunning

    public UnityEvent OnStartedWallRunningRight;
    public UnityEvent OnStartedWallRunningLeft;


    private bool IsGrounded;
    public bool IsWallRight { get; private set; }
    public bool IsWallLeft { get; private set; }


    private bool isTouchingWallOnRight;
    private bool isTouchingWallOnLeft;

    private bool isWallRunningOnRightWall;
    private bool isWallRunningOnLeftWall;

    private bool isWallRunning;

    private Vector3 wallNormal;

    private CapsuleCollider _collider;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _camera = GetComponent<PlayerInput>().camera;

        _groundedManager = GetComponent<GroundedManager>();

        _collider = GetComponent<CapsuleCollider>();
    }

    private void FixedUpdate()
    {
        UpdateRotationBasedOnCamera();

        if (!_groundedManager.IsGrounded)
        {
            ApplyCustomGravity(gravityScale);
        }

        CheckIfIsWallRunning();

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

    private void OnCollisionEnter(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {

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
        var cameraRotation = _camera.transform.rotation.eulerAngles;
        var cameraYRotation = Quaternion.Euler(0, cameraRotation.y, 0);
        _rigidbody.rotation = cameraYRotation;
    }

    private void ApplyCustomGravity(float gravityScale)
    {
        var gravity = Physics.gravity * (gravityScale - 1);
        _rigidbody.AddForce(gravity, ForceMode.Acceleration);
    }


    private void CheckIfIsWallRunning()
    {
        isWallRunningOnRightWall = isTouchingWallOnRight && !IsGrounded;
        isWallRunningOnLeftWall = isTouchingWallOnLeft && !IsGrounded;

        isWallRunning = isWallRunningOnRightWall || isWallRunningOnLeftWall;
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
