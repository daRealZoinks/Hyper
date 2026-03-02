using Hyper.Player;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(LineRenderer))]
public class GrapplingGun : MonoBehaviour
{
    public float maxDistance = 100f;
    public float grapplePower = 1f;
    public float dampingFactor = 5f;
    public float maxForce = 100f;

    public RigidbodyCharacterController characterController;

    private float distanceFromPoint;
    private Vector3? grapplePoint;

    private Camera _camera;
    private LineRenderer _lineRenderer;

    private Rigidbody _playerRigidbody;
    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _lineRenderer = GetComponent<LineRenderer>();

        _playerRigidbody = characterController.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (grapplePoint.HasValue)
        {
            _lineRenderer.SetPosition(0, transform.position + Vector3.down);
            _lineRenderer.SetPosition(1, grapplePoint.Value);
        }
    }

    private void FixedUpdate()
    {
        if (grapplePoint.HasValue)
        {
            var toPoint = grapplePoint.Value - _playerRigidbody.position;
            var currentDistance = toPoint.magnitude;

            if (currentDistance > 0.001f)
            {
                var displacement = currentDistance - distanceFromPoint;

                if (displacement > 0f)
                {
                    var springForce = toPoint.normalized * (displacement * grapplePower);

                    var radialVelocity = Vector3.Dot(_playerRigidbody.linearVelocity, toPoint.normalized);
                    var dampingForce = -toPoint.normalized * (radialVelocity * dampingFactor);

                    var totalForce = springForce + dampingForce;

                    totalForce = Vector3.Min(totalForce, totalForce.normalized * maxForce);

                    _playerRigidbody.AddForce(totalForce, ForceMode.Acceleration);
                }
            }
        }
    }

    public void StartGrapple()
    {
        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out var hit, maxDistance))
        {
            grapplePoint = hit.point;

            distanceFromPoint = Vector3.Distance(transform.position, grapplePoint.Value);

            _lineRenderer.enabled = true;
        }
    }

    public void StopGrapple()
    {
        _lineRenderer.enabled = false;

        grapplePoint = null;
    }

    private void OnDisable()
    {
        StopGrapple();
    }
}
