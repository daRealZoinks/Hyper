using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public float acceleration = 60f;
    public float topSpeed = 8f;
    public float deceleration = 120f;

    public float airControl = 0.5f;
    public float airBreak = 0f;

    private Rigidbody _rigidbody;
    private GroundedManager _groundedManager;
    private RigidbodyCharacterController _rigidbodyCharacterController;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _groundedManager = GetComponent<GroundedManager>();
        _rigidbodyCharacterController = GetComponent<RigidbodyCharacterController>();
    }

    private void FixedUpdate()
    {
        Move(_rigidbodyCharacterController.currentInputPayload.MoveInput);
    }

    public void Move(Vector2 moveInput)
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

        if (!_groundedManager.IsGrounded)
        {
            finalForce *= (inputDirection != Vector3.zero) ? airControl : airBreak;
        }

        _rigidbody.AddForce(finalForce, ForceMode.Acceleration);
    }
}
