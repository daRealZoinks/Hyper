using UnityEngine;

[RequireComponent(typeof(ThrowingBall))]
public class RockHyperBall : MonoBehaviour
{
    public float knockbackForce = 25f;
    public float upwardKnockbackForce = 3f;
    public float knockbackAngle = 60f;

    private void Start()
    {
        GetComponent<ThrowingBall>().OnTargetHit += ThrowingBall_OnTargetHit;
    }

    private void ThrowingBall_OnTargetHit(Rigidbody origin, Rigidbody target, float spinAngle)
    {
        var hitAngle = spinAngle > 0 ? knockbackAngle : -knockbackAngle;

        var knockbackDirection = Quaternion.Euler(0, hitAngle, 0) * (target.position - origin.position).normalized;

        Debug.DrawLine(target.position, target.position + knockbackDirection, Color.red, 5f);
        target.AddForce(knockbackDirection * knockbackForce + Vector3.up * upwardKnockbackForce, ForceMode.VelocityChange);
    }
}
