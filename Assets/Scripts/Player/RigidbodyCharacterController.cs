using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyCharacterController : MonoBehaviour
{
    // public variables
    public float acceleration = 60f;
    public float topSpeed = 8f;
    public float deceleration = 120f;

    public float airControl = 0.25f;
    public float airBreak = 0f;

    public float jumpHeight = 2f;
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.15f;

    public float gravityScale = 1.5f;
    public float slopeLimit = 45f;

    // public events
    public UnityEvent OnLanded;
    public UnityEvent OnJump;



    // public properties
    public Vector2 MoveInput { private get; set; }
    public bool Sliding { private get; set; }






    // private variables
    private bool isGrounded;
    private Vector3 groundNormal;

    private float _jumpBufferCounter;
    private float _coyoteTimeCounter;




    // private references to components
    private Rigidbody _rigidbody;





    // private references to other objects
    [SerializeField]
    private Camera _camera;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!isGrounded)
        {
            ApplyCustomGravity(gravityScale);
        }

        UpdateRotationBasedOnCamera();

        Move(MoveInput);

        UpdateJumpBufferCounter();
        UpdateCoyoteTimeCounter();
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (var contactPoint in collision.contacts)
        {
            var angle = Vector3.Angle(contactPoint.normal, Vector3.up);

            if (angle <= slopeLimit)
            {
                if (!isGrounded)
                {
                    isGrounded = true;
                    groundNormal = contactPoint.normal;
                    OnLanded?.Invoke();
                    break;
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (var contactPoint in collision.contacts)
        {
            var angle = Vector3.Angle(contactPoint.normal, Vector3.up);

            if (angle <= slopeLimit)
            {
                isGrounded = true;
                groundNormal = contactPoint.normal;
                break;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    private void ApplyCustomGravity(float gravityScale)
    {
        _rigidbody.AddForce(Physics.gravity * gravityScale, ForceMode.Acceleration);
    }

    private void UpdateRotationBasedOnCamera()
    {
        var cameraForward = _camera.transform.forward;
        cameraForward.y = 0;
        _rigidbody.rotation = Quaternion.LookRotation(cameraForward.normalized);
    }

    private void Move(Vector2 moveInput)
    {
        var inputDirection = transform.right * moveInput.x + transform.forward * moveInput.y;

        var horizontalRigidbodyVelocity = new Vector3
        {
            x = _rigidbody.linearVelocity.x,
            z = _rigidbody.linearVelocity.z
        };

        var horizontalClampedVelocity = horizontalRigidbodyVelocity.normalized * Mathf.Clamp01(horizontalRigidbodyVelocity.magnitude / topSpeed);

        var finalForce = inputDirection - horizontalClampedVelocity;

        finalForce *= (inputDirection != Vector3.zero) ? acceleration : deceleration;

        if (isGrounded)
        {
            finalForce = Vector3.ProjectOnPlane(finalForce, groundNormal);
        }
        else
        {
            // TODO: make better 1 - default transitions between airControl and airBreak 
            finalForce *= inputDirection != Vector3.zero ? airControl : airBreak;
        }

        _rigidbody.AddForce(finalForce, ForceMode.Acceleration);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            ExecuteJump();
            OnJump?.Invoke();
            _coyoteTimeCounter = 0f;
        }
    }

    private void ExecuteJump()
    {
        var jumpForce = Vector3.up * Mathf.Sqrt(-2f * Physics.gravity.y * gravityScale * jumpHeight);

        if (_rigidbody.linearVelocity.y < 0)
        {
            _rigidbody.linearVelocity = new Vector3()
            {
                x = _rigidbody.linearVelocity.x,
                z = _rigidbody.linearVelocity.z
            };
        }

        _rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);
    }

    private void UpdateJumpBufferCounter()
    {
        if (_jumpBufferCounter > 0f)
        {
            _jumpBufferCounter -= Time.fixedDeltaTime;
        }
    }

    private void UpdateCoyoteTimeCounter()
    {
        if (_coyoteTimeCounter > 0f)
        {
            _coyoteTimeCounter -= Time.fixedDeltaTime;
        }
    }
}
