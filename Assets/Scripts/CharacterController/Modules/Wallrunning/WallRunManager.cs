using UnityEngine;
using UnityEngine.Events;

public class WallRunManager : MonoBehaviour
{

    public float wallDetectionAngleThreshold = 0.9f;

    public float wallRunInitialImpulse = 5f;


    public float wallRunGravity = 0.75f; // Lower gravity while wallrunning



    public UnityEvent OnStartedWallRunningRight;
    public UnityEvent OnStartedWallRunningLeft;

    public bool IsWallRunningOnRightWall => isTouchingWallOnRight && !_groundedManager.IsGrounded;
    public bool IsWallRunningOnLeftWall => isTouchingWallOnLeft && !_groundedManager.IsGrounded;

    public bool IsWallRunning => IsWallRunningOnLeftWall || IsWallRunningOnRightWall;


    private bool isTouchingWallOnRight;
    private bool isTouchingWallOnLeft;


    private Vector3 wallNormal;

    private Vector3 topCollisionPoint;
    private Vector3 middleCollisionPoint;

    private GroundedManager _groundedManager;
    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;

    private void Awake()
    {
        _groundedManager = GetComponent<GroundedManager>();
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        topCollisionPoint = _rigidbody.position + _collider.center + Vector3.up * (_collider.height / 2);
        middleCollisionPoint = _rigidbody.position + _collider.center + Vector3.up * 0.1f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            if (contact.point.y <= topCollisionPoint.y && contact.point.y >= middleCollisionPoint.y)
            {
                isTouchingWallOnRight = Vector3.Dot(contact.normal, -transform.right) > wallDetectionAngleThreshold;
                isTouchingWallOnLeft = Vector3.Dot(contact.normal, transform.right) > wallDetectionAngleThreshold;

                if (!_groundedManager.IsGrounded)
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
        foreach (var contact in collision.contacts)
        {
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
        isTouchingWallOnRight = false;
        isTouchingWallOnLeft = false;

        wallNormal = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (IsWallRunning)
        {
            _rigidbody.AddForce(-Physics.gravity * wallRunGravity, ForceMode.Acceleration);
        }
    }
}
