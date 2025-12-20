using Hyper.Player;
using UnityEngine;

namespace Hyper.HyperBalls
{
    public class YoyoLine : MonoBehaviour
    {
        public float speed = 30f;
        public float rangeToDisengage = 3f;
        public float maxLifetime = 10f;
        public LineRenderer lineRenderer;

        public Vector3? TargetPoint { private get; set; } = null;
        public RigidbodyCharacterController TargetCharacterController { private get; set; }
        public Rigidbody OwningPlayerRigidbody { private get; set; }


        private void Awake()
        {
            Destroy(gameObject, maxLifetime);
        }

        private void LateUpdate()
        {
            if (TargetPoint.HasValue)
            {
                lineRenderer.SetPosition(0, TargetPoint.Value - transform.position);
                lineRenderer.SetPosition(1, OwningPlayerRigidbody.position - transform.position);
            }

            if (TargetCharacterController)
            {
                var targetRigidbody = TargetCharacterController.GetComponent<Rigidbody>();
                lineRenderer.SetPosition(0, targetRigidbody.position - transform.position);
                lineRenderer.SetPosition(1, OwningPlayerRigidbody.position - transform.position);
            }
        }

        private void FixedUpdate()
        {
            if (TargetPoint.HasValue)
            {
                var owningPlayerRigidbodyCharacterController = OwningPlayerRigidbody.GetComponent<RigidbodyCharacterController>();
                var toTarget = TargetPoint.Value - OwningPlayerRigidbody.position;

                if (toTarget.magnitude > rangeToDisengage)
                {
                    owningPlayerRigidbodyCharacterController.useGravity = false;
                    OwningPlayerRigidbody.linearVelocity = toTarget.normalized * speed;
                }
                else
                {
                    owningPlayerRigidbodyCharacterController.useGravity = true;
                    Destroy(gameObject);
                }
            }

            if (TargetCharacterController)
            {
                var owningPlayerRigidbodyCharacterController = OwningPlayerRigidbody.GetComponent<RigidbodyCharacterController>();
                var targetRigidbody = TargetCharacterController.GetComponent<Rigidbody>();
                var toTarget = targetRigidbody.position - OwningPlayerRigidbody.position;

                if (toTarget.magnitude > rangeToDisengage)
                {
                    owningPlayerRigidbodyCharacterController.useGravity = false;
                    TargetCharacterController.useGravity = false;
                    OwningPlayerRigidbody.linearVelocity = toTarget.normalized * speed;
                    targetRigidbody.linearVelocity = -toTarget.normalized * speed;
                }
                else
                {
                    owningPlayerRigidbodyCharacterController.useGravity = true;
                    TargetCharacterController.useGravity = true;
                    Destroy(gameObject);
                }
            }
        }
    }
}