using UnityEngine;

namespace Hyper.HyperBalls
{
    public class HyperBall : MonoBehaviour
    {
        public float lifeTime = 15f;

        public Rigidbody OwningPlayerRigidbody { get; set; }

        public Rigidbody TargetPlayerRigidbody { get; set; }

        private void Awake()
        {
            Destroy(gameObject, lifeTime);
        }

        private void FixedUpdate()
        {
            // TODO: add trajectory physics here if needed to go towards the target 
        }
    }
}
