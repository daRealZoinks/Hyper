using Hyper.Player;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(LineRenderer))]
public class GrapplingGun : MonoBehaviour
{
    public float maxDistance = 40f;
    public float grappleSpeed = 100f; // m/s

    public float maxDistanceFromPoint = 0.8f;
    public float minDistanceFromPoint = 0f;

    public float selectionBoxRatio = 0.75f;

    public float spring = 4.5f;
    public float damper = 7f;
    public float massScale = 4.5f;

    public RigidbodyCharacterController characterController;

    public LayerMask grapplePointLayerMask;

    private GrapplePoint _candidate;
    private GrapplePoint _grapplePoint;
    private GrapplePoint _targetGrapplePoint; // the point we're shooting the hook to

    private bool _isHookShot = false;
    private bool _isHookRetracting = false;
    private Vector3 _hookPosition;

    private Camera _camera;
    private LineRenderer _lineRenderer;
    private SpringJoint _springJoint;

    private Rigidbody _playerRigidbody;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = 2;
        _lineRenderer.enabled = false;

        _playerRigidbody = characterController.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_isHookShot || _isHookRetracting)
        {
            if (_isHookShot)
            {
                _isHookRetracting = false;
                var step = grappleSpeed * Time.deltaTime;
                _hookPosition = Vector3.MoveTowards(_hookPosition, _targetGrapplePoint.transform.position, step);

                if (Vector3.Distance(_hookPosition, _targetGrapplePoint.transform.position) <= 0.25f)
                {
                    AttachGrapple();
                }
            }

            if (_isHookRetracting)
            {
                _isHookShot = false;
                var step = grappleSpeed * Time.deltaTime;
                _hookPosition = Vector3.MoveTowards(_hookPosition, transform.position, step);

                if (Vector3.Distance(_hookPosition, transform.position) <= 0.25f)
                {
                    _isHookRetracting = false;
                    _lineRenderer.enabled = false;
                }
            }
        }
        else
        {
            _candidate = FindClosestGrapplePointInSelectionCircle();

            if (_grapplePoint != null)
            {
                _hookPosition = _grapplePoint.transform.position;
            }
        }

        RenderLine();
    }

    private void OnGUI()
    {
        // draw text for the candidate grapple point
        if (_candidate != null && _candidate != _grapplePoint)
        {
            var screenPoint = _camera.WorldToScreenPoint(_candidate.transform.position);
            if (screenPoint.z > 0f)
            {
                var label = $"Candidate: {_candidate.name}";
                var labelSize = GUI.skin.label.CalcSize(new GUIContent(label));
                var labelRect = new Rect(screenPoint.x - labelSize.x * 0.5f, Screen.height - screenPoint.y - labelSize.y, labelSize.x, labelSize.y);
                GUI.Label(labelRect, label);
            }
        }

        // draw text for the current grapple point
        if (_grapplePoint != null)
        {
            var screenPoint = _camera.WorldToScreenPoint(_grapplePoint.transform.position);
            if (screenPoint.z > 0f)
            {
                var label = $"Grappling: {_grapplePoint.name}";
                var labelSize = GUI.skin.label.CalcSize(new GUIContent(label));
                var labelRect = new Rect(screenPoint.x - labelSize.x * 0.5f, Screen.height - screenPoint.y - labelSize.y, labelSize.x, labelSize.y);
                GUI.Label(labelRect, label);
            }
        }
    }

    private void RenderLine()
    {
        _lineRenderer.SetPosition(0, transform.position + Vector3.down);
        _lineRenderer.SetPosition(1, _hookPosition);
    }

    public void StartGrapple()
    {
        _grapplePoint = null;

        if (_springJoint != null)
        {
            Destroy(_springJoint);
        }

        if (_candidate != null)
        {
            _targetGrapplePoint = _candidate;
            _isHookShot = true;
            _hookPosition = transform.position;

            _lineRenderer.enabled = true;
        }
    }

    private void AttachGrapple()
    {
        if (_targetGrapplePoint == null)
        {
            _isHookShot = false;
            _lineRenderer.enabled = false;
            return;
        }

        _grapplePoint = _targetGrapplePoint;
        _targetGrapplePoint = null;
        _isHookShot = false;

        _springJoint = _playerRigidbody.gameObject.AddComponent<SpringJoint>();
        _springJoint.autoConfigureConnectedAnchor = false;
        _springJoint.connectedAnchor = _grapplePoint.transform.position;

        var distanceFromPoint = Vector3.Distance(transform.position, _grapplePoint.transform.position);

        _springJoint.maxDistance = distanceFromPoint * maxDistanceFromPoint;
        _springJoint.minDistance = distanceFromPoint * minDistanceFromPoint;

        _springJoint.spring = spring;
        _springJoint.damper = damper;
        _springJoint.massScale = _playerRigidbody.mass * characterController.gravityScale * massScale;

        _lineRenderer.enabled = true;
    }

    private GrapplePoint FindClosestGrapplePointInSelectionCircle()
    {
        var results = new Collider[10];

        var overlapSphereHitNumber = Physics.OverlapSphereNonAlloc(_camera.transform.position, maxDistance, results, grapplePointLayerMask);

        var colliders = results.Take(overlapSphereHitNumber);

        if (colliders == null || colliders.Count() == 0)
        {
            return null;
        }

        var grapplePoints = colliders
            .Where(collider => collider != null)
            .Where(collider =>
            {
                if (Physics.Raycast(_camera.transform.position, collider.transform.position - _camera.transform.position, out var hitInfo))
                {
                    if (collider.gameObject == hitInfo.collider.gameObject)
                    {
                        return true;
                    }
                }

                return false;
            })
            .Select(collider => collider.GetComponent<GrapplePoint>())
            .Where(gp => gp != null)
            .ToList();

        if (grapplePoints.Count == 0)
        {
            return null;
        }

        var viewPort = _camera.pixelRect;
        var half = (float)(Mathf.Min(viewPort.width, viewPort.height) * selectionBoxRatio) * 0.5f;

        var screenCenter = new Vector2(viewPort.x + viewPort.width * 0.5f, viewPort.y + viewPort.height * 0.5f);

        GrapplePoint bestGrapplePoint = null;
        var bestDistanceSquared = float.MaxValue;

        foreach (var grapplePoint in grapplePoints)
        {
            var screenPoint = _camera.WorldToScreenPoint(grapplePoint.transform.position);
            if (screenPoint.z < 0f)
            {
                continue;
            }

            var distanceFromTheCenter = Vector2.Distance(screenPoint, screenCenter);

            if (distanceFromTheCenter <= half)
            {
                if (distanceFromTheCenter < bestDistanceSquared)
                {
                    bestDistanceSquared = distanceFromTheCenter;
                    bestGrapplePoint = grapplePoint;
                }
            }
        }

        return bestGrapplePoint;
    }

    public void StopGrapple()
    {
        _grapplePoint = null;
        _isHookRetracting = true;

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
