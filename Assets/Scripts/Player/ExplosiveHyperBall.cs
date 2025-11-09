using UnityEngine;

namespace Hyper.Player
{
    [RequireComponent(typeof(HyperBall))]
    public class ExplosiveHyperBall : MonoBehaviour
    {
        public float explosionRadius = 5f;
        public float explosionForce = 100f;
        public float upwardsModifier = 1f;

        private HyperBall _hyperBall;

        private void Awake()
        {
            _hyperBall = GetComponent<HyperBall>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            Explode();
            Destroy(gameObject);
        }


        private void Explode()
        {
            var detectedColliders = new Collider[100];
            var size = Physics.OverlapSphereNonAlloc(transform.position, explosionRadius, detectedColliders);

            for (var i = 0; i < size; i++)
            {
                var hitRigidbody = detectedColliders[i].attachedRigidbody;

                if (hitRigidbody)
                {
                    hitRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier, ForceMode.VelocityChange);
                }
            }

        }
    }
}