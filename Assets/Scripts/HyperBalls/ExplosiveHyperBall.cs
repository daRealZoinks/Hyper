using UnityEngine;

namespace Hyper.HyperBalls
{
    [RequireComponent(typeof(HyperBall))]
    public class ExplosiveHyperBall : MonoBehaviour
    {
        public float explosionRadius = 5f;
        public float explosionForce = 20f;
        public float upwardsModifier = 20f;
        public float linearVelocityModifier = 50f;

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
                    var explosionForceDirection = (hitRigidbody.position - transform.position).normalized;

                    var forceDirection = (explosionForceDirection * ((100 - upwardsModifier) / 100)).normalized +
                        (Vector3.up * (upwardsModifier / 100)).normalized;

                    if (hitRigidbody == _hyperBall.OwningPlayerRigidbody)
                    {
                        var linearVelocity = new Vector3()
                        {
                            x = hitRigidbody.linearVelocity.x,
                            z = hitRigidbody.linearVelocity.z,
                        };

                        forceDirection = ((forceDirection * ((100 - linearVelocityModifier) / 100)).normalized +
                            (linearVelocity * (linearVelocityModifier / 100)).normalized).normalized;
                    }

                    hitRigidbody.linearVelocity = forceDirection.normalized * explosionForce;
                }
            }
        }
    }
}