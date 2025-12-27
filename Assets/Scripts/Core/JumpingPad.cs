using Hyper.Player;
using UnityEngine;

namespace Hyper.Core
{
    public class JumpingPad : MonoBehaviour
    {
        public Vector3 jumpForce = new(0, 25, 0);

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent<RigidbodyCharacterController>(out var rigidbodyCharacterController))
            {
                if (rigidbodyCharacterController)
                {
                    var rigidbody = rigidbodyCharacterController.GetComponent<Rigidbody>();

                    if (rigidbody)
                    {
                        var jumpVector = new Vector3()
                        {
                            x = jumpForce.x,
                            y = Mathf.Sqrt(2 * Physics.gravity.magnitude * rigidbodyCharacterController.gravityScale * jumpForce.y),
                            z = jumpForce.z
                        };

                        rigidbody.linearVelocity = jumpVector;
                    }
                }
            }
        }
    }
}