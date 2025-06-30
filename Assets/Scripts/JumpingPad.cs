using UnityEngine;

public class JumpingPad : MonoBehaviour
{
    public Vector3 jumpForce = new(0, 25, 0);

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<GravityModule>(out var gravityModule))
        {
            if (gravityModule)
            {
                var rigidbody = gravityModule.GetComponent<Rigidbody>();

                if (rigidbody)
                {
                    var jumpVector = jumpForce;
                    jumpVector.y = Mathf.Sqrt(2 * Physics.gravity.magnitude * gravityModule.defaultGravityScale * jumpVector.y);

                    rigidbody.linearVelocity = jumpVector;
                }
            }
        }
    }
}
