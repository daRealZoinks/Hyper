using UnityEngine;

namespace Hyper.Player
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
        }

        private void FixedUpdate()
        {
            if (TargetPoint.HasValue)
            {
                var toTarget = TargetPoint.Value - OwningPlayerRigidbody.position;
                var owningPlayerRigidbodyCharacterController = OwningPlayerRigidbody.GetComponent<RigidbodyCharacterController>();

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
        }
    }
}