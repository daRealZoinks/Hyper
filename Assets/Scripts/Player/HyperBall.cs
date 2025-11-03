using Unity.Netcode;
using UnityEngine;

namespace Hyper.Player
{
    public class HyperBall : NetworkBehaviour
    {
        public float lifeTime = 15f;

        public Rigidbody OwningPlayerRigidbody { get; set; }

        public Rigidbody TargetPlayerRigidbody { get; set; }

        private void FixedUpdate()
        {
            // TODO: add trajectory physics here if needed to go towards the target 
        }

        public override void OnNetworkSpawn()
        {
            Destroy(gameObject, lifeTime);
        }
    }
}
