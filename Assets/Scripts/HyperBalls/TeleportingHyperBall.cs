using Hyper.Player;
using UnityEngine;

namespace Hyper.HyperBalls
{
    [RequireComponent(typeof(HyperBall))]
    public class TeleportingHyperBall : MonoBehaviour
    {
        private HyperBall _hyperBall;

        private void Awake()
        {
            _hyperBall = GetComponent<HyperBall>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            var contactPoint = collision.contacts[0];

            var hitPlayerRigidbodyCharacterController = collision.gameObject.GetComponentInChildren<RigidbodyCharacterController>();

            if (hitPlayerRigidbodyCharacterController)
            {
                var hitPlayerRigidbody = hitPlayerRigidbodyCharacterController.GetComponent<Rigidbody>();
                (_hyperBall.OwningPlayerRigidbody.position, hitPlayerRigidbody.position) = (hitPlayerRigidbody.position, _hyperBall.OwningPlayerRigidbody.position);
            }
            else
            {
                _hyperBall.OwningPlayerRigidbody.position = contactPoint.point;
            }

            Destroy(gameObject);
        }
    }
}