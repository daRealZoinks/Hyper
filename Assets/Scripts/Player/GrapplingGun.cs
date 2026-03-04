using Hyper.Player;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(LineRenderer))]
public class GrapplingGun : MonoBehaviour
{
    public float maxDistance = 100f;

    public float maxDistanceFromPoint = 0.8f;
    public float minDistanceFromPoint = 0.25f;

    public float spring = 4.5f;
    public float damper = 7f;
    public float massScale = 4.5f;

    public RigidbodyCharacterController characterController;

    private Vector3? grapplePoint;

    private Camera _camera;
    private LineRenderer _lineRenderer;
    private SpringJoint _springJoint;

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

    public void StartGrapple()
    {
        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out var hit, maxDistance))
        {
            grapplePoint = hit.point;

            _springJoint = _playerRigidbody.gameObject.AddComponent<SpringJoint>();
            _springJoint.autoConfigureConnectedAnchor = false;
            _springJoint.connectedAnchor = grapplePoint.Value;

            var distanceFromPoint = Vector3.Distance(transform.position, grapplePoint.Value);

            _springJoint.maxDistance = distanceFromPoint * maxDistanceFromPoint;
            _springJoint.minDistance = distanceFromPoint * minDistanceFromPoint;

            _springJoint.spring = spring;
            _springJoint.damper = damper;
            _springJoint.massScale = _playerRigidbody.mass * characterController.gravityScale * massScale;

            _lineRenderer.enabled = true;
        }
    }

    public void StopGrapple()
    {
        _lineRenderer.enabled = false;

        grapplePoint = null;

        if (_springJoint != null)
        {
            Destroy(_springJoint);
        }
    }

    private void OnDisable()
    {
        StopGrapple();
    }
}
