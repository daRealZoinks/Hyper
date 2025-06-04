using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    public bool GrapplingEnabled { get; internal set; }

    public int maxDistance = 100;
    public new Transform camera;
    public GameObject player;

    public float maxDistanceFromPoint = 0.8f;
    public float minDistanceFromPoint = 0.25f;
    public float spring = 4.5f;
    public float damper = 7f;
    public float massScale = 4.5f;

    private LineRenderer lineRenderer;
    private Vector3 grapplePoint;
    private SpringJoint springJoint;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        StopGrapple();
    }

    private void Update()
    {
        if (!GrapplingEnabled)
        {
            StopGrapple();
        }
    }

    private void LateUpdate()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }

    public void StartGrapple()
    {
        if (!GrapplingEnabled) return;

        if (Physics.Raycast(camera.position, camera.forward, out var hit, maxDistance))
        {
            grapplePoint = hit.point;

            springJoint = player.AddComponent<SpringJoint>();
            springJoint.autoConfigureConnectedAnchor = false;
            springJoint.connectedAnchor = grapplePoint;

            var distanceFromPoint = Vector3.Distance(player.transform.position, grapplePoint);

            springJoint.maxDistance = distanceFromPoint * maxDistanceFromPoint;
            springJoint.minDistance = distanceFromPoint * minDistanceFromPoint;

            springJoint.spring = spring;
            springJoint.damper = damper;
            springJoint.massScale = massScale;

            lineRenderer.enabled = true;
        }
    }

    public void StopGrapple()
    {
        lineRenderer.enabled = false;

        if (springJoint != null)
        {
            Destroy(springJoint);
        }
    }
}
