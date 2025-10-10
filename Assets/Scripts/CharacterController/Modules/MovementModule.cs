using UnityEngine;

public class MovementModule : MonoBehaviour
{
    [SerializeField]
    private float airControl = 0.25f;
    [SerializeField]
    private float airBreak = 0f;

    [HideInInspector]
    public float currentAcceleration;
    [HideInInspector]
    public float currentTopSpeed;
    [HideInInspector]
    public float currentDeceleration;

    private Rigidbody _rigidbody;
    private GroundCheckModule _groundCheckModule;
    private SlidingModule _slidingModule;
    private GroundMovementModule _groundMovementModule;
    private OldRigidbodyCharacterController _rigidbodyCharacterController;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _groundCheckModule = GetComponent<GroundCheckModule>();
        _slidingModule = GetComponent<SlidingModule>();
        _groundMovementModule = GetComponent<GroundMovementModule>();
        _rigidbodyCharacterController = GetComponent<OldRigidbodyCharacterController>();
    }

    private void FixedUpdate()
    {
        if (_slidingModule.IsSliding)
        {
            currentAcceleration = _slidingModule.acceleration;
            currentTopSpeed = _slidingModule.topSpeed;
            currentDeceleration = _slidingModule.deceleration;
        }
        else
        {
            currentAcceleration = _groundMovementModule.acceleration;
            currentTopSpeed = _groundMovementModule.topSpeed;
            currentDeceleration = _groundMovementModule.deceleration;
        }

        Move(_rigidbodyCharacterController.CurrentInputPayload.MoveInput);
    }

    public void Move(Vector2 moveInput)
    {
        var inputDirection = transform.right * moveInput.x + transform.forward * moveInput.y;

        var horizontalRigidbodyVelocity = new Vector3
        {
            x = _rigidbody.linearVelocity.x,
            z = _rigidbody.linearVelocity.z
        };

        var horizontalClampedVelocity = horizontalRigidbodyVelocity.normalized * Mathf.Clamp01(horizontalRigidbodyVelocity.magnitude / currentTopSpeed);

        var finalForce = inputDirection - horizontalClampedVelocity;

        finalForce *= (inputDirection != Vector3.zero) ? currentAcceleration : currentDeceleration;

        if (_groundCheckModule.IsGrounded)
        {
            finalForce = Vector3.ProjectOnPlane(finalForce, _groundCheckModule.GroundNormal);
        }
        else
        {
            finalForce *= inputDirection != Vector3.zero ? airControl : airBreak;
        }

        if (inputDirection == Vector3.zero && horizontalRigidbodyVelocity.magnitude < 0.05)
        {
            _rigidbody.linearVelocity = Vector3.up * _rigidbody.linearVelocity.y;
        }
        else
        {
            if (!float.IsNaN(finalForce.x) && !float.IsNaN(finalForce.y) && !float.IsNaN(finalForce.z))
            {
                _rigidbody.AddForce(finalForce, ForceMode.Acceleration);
            }
        }
    }
}
