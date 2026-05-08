using UnityEngine;

[RequireComponent(typeof(ThrowingBall))]
public class YoyoHyperBall : MonoBehaviour
{
    private void Start()
    {
        GetComponent<ThrowingBall>().OnTargetHit += ThrowingBall_OnTargetHit;
    }

    private void ThrowingBall_OnTargetHit(Rigidbody origin, Rigidbody target, float _)
    {
        // Implement Yoyo behavior here
    }
}
