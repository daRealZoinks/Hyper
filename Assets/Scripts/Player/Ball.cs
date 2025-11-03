using Unity.Netcode;
using UnityEngine;

namespace Hyper.Player
{
    public class Ball : NetworkBehaviour
    {
        public float lifeTime = 15f;

        public Rigidbody PlayerRigidbody { private get; set; }

        public override void OnNetworkSpawn()
        {
            Destroy(gameObject, lifeTime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            var contactPoint = collision.contacts[0];

            var hitPlayerRigidbodyCharacterController = collision.gameObject.GetComponentInChildren<RigidbodyCharacterController>();

            if (hitPlayerRigidbodyCharacterController)
            {
                var hitPlayerRigidbody = hitPlayerRigidbodyCharacterController.GetComponent<Rigidbody>();
                (PlayerRigidbody.position, hitPlayerRigidbody.position) = (hitPlayerRigidbody.position, PlayerRigidbody.position);
            }
            else
            {
                PlayerRigidbody.position = contactPoint.point;
            }

            Destroy(gameObject);
        }
    }
}
