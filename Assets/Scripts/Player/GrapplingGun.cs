using Hyper.Player;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(LineRenderer))]
public class GrapplingGun : MonoBehaviour
{
    public float maxDistance = 100f;

    public float maxDistanceFromPoint = 0.8f;
    public float minDistanceFromPoint = 0f;

    // Fraction of the camera viewport's smaller dimension used for the
    // square selection box (0..1). This makes the selection box work
    // correctly for split-screen when each player uses their own camera.
    // e.g. 0.75f uses 75% of the smaller viewport dimension for the box size
    public float selectionBoxRatio = 0.75f;

    public float spring = 4.5f;
    public float damper = 7f;
    public float massScale = 4.5f;

    public RigidbodyCharacterController characterController;

    private GrapplePoint _candidate;
    private GrapplePoint _grapplePoint;

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
        _candidate = FindClosestGrapplePointInSelectionBox();

        RenderLine();
    }

    private void OnGUI()
    {
        // Use the camera's viewport (pixelRect) so split-screen cameras work
        var vp = _camera.pixelRect;
        var vpWidth = vp.width;
        var vpHeight = vp.height;
        var baseSize = Mathf.Min(vpWidth, vpHeight);
        var boxSize = baseSize * selectionBoxRatio;
        var half = boxSize * 0.5f;

        var centerX = vp.x + vpWidth * 0.5f;
        var centerY = vp.y + vpHeight * 0.5f; // origin is bottom-left

        // GUI coordinates origin is top-left, so convert Y
        var rectX = centerX - half;
        var rectY = Screen.height - (centerY + half);
        var rect = new Rect(rectX, rectY, boxSize, boxSize);
        GUI.Box(rect, "Grapple Point Selection Box");

        // draw text for the candidate grapple point
        if (_candidate != null)
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
        if (_grapplePoint)
        {
            _lineRenderer.SetPosition(0, transform.position + Vector3.down);
            _lineRenderer.SetPosition(1, _grapplePoint.transform.position);
        }
    }

    public void StartGrapple()
    {
        StopGrapple();

        // Find the best grapple point candidate from GrapplePoint objects that
        // are inside a screen-space square centered on the screen. Pick the one
        // closest to the screen center.
        if (_candidate != null)
        {
            // Respect maximum world distance to the grapple point
            if (Vector3.Distance(_playerRigidbody.position, _candidate.transform.position) <= maxDistance)
            {
                _grapplePoint = _candidate;

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
        }
    }

    private GrapplePoint FindClosestGrapplePointInSelectionBox()
    {
        var allGrapplePointsOnTheMap = FindObjectsByType<GrapplePoint>(FindObjectsSortMode.None);

        if (allGrapplePointsOnTheMap == null || allGrapplePointsOnTheMap.Length == 0)
        {
            return null;
        }

        var grapplePoints = allGrapplePointsOnTheMap.Where(gp => Vector3.Distance(_playerRigidbody.position, gp.transform.position) <= maxDistance);

        var viewPort = _camera.pixelRect;
        var baseSize = Mathf.Min(viewPort.width, viewPort.height);
        var boxSize = baseSize * selectionBoxRatio;
        var half = boxSize * 0.5f;

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

            var delta = new Vector2(screenPoint.x - screenCenter.x, screenPoint.y - screenCenter.y);

            if (Mathf.Abs(delta.x) <= half && Mathf.Abs(delta.y) <= half)
            {
                var distanceSquared = delta.sqrMagnitude;
                if (distanceSquared < bestDistanceSquared)
                {
                    bestDistanceSquared = distanceSquared;
                    bestGrapplePoint = grapplePoint;
                }
            }
        }

        return bestGrapplePoint;
    }

    public void StopGrapple()
    {
        _lineRenderer.enabled = false;

        _grapplePoint = null;

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
