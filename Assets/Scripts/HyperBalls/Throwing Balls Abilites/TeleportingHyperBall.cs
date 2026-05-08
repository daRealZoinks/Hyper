using UnityEngine;

[RequireComponent(typeof(ThrowingBall))]
public class TeleportingHyperBall : MonoBehaviour
{
    private void Start()
    {
        GetComponent<ThrowingBall>().OnTargetHit += ThrowingBall_OnTargetHit;
    }

    private void ThrowingBall_OnTargetHit(Rigidbody origin, Rigidbody target, float _)
    {
        (origin.position, target.position) = (target.position, origin.position);
    }
}
