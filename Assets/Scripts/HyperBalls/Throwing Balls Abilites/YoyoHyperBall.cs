using Hyper.HyperBalls;
using UnityEngine;

[RequireComponent(typeof(ThrowingBall))]
public class YoyoHyperBall : MonoBehaviour
{
    public YoyoLine yoyoLinePrefab;

    public LineRenderer lineRenderer;

    public Transform OriginPlayer { get; private set; }

    private void LateUpdate()
    {
        lineRenderer.SetPosition(0, OriginPlayer.position - transform.position);
        lineRenderer.SetPosition(1, Vector3.zero);
    }

    private void Start()
    {
        var throwingBall = GetComponent<ThrowingBall>();

        OriginPlayer = throwingBall.OriginPlayerTransform;
        throwingBall.OnTargetHit += ThrowingBall_OnTargetHit;
    }

    private void ThrowingBall_OnTargetHit(Rigidbody origin, Rigidbody target, float _)
    {
        var yoyoLineInstance = Instantiate(yoyoLinePrefab, target.position, Quaternion.identity);
        yoyoLineInstance.OwnerRigidbody = origin;
        yoyoLineInstance.TargetRigidbody = target;
    }
}
