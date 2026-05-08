using Hyper.Player;
using UnityEngine;

namespace Hyper.HyperBalls
{
    [RequireComponent(typeof(HyperBall))]
    public class YoyoHyperBallPrototype : MonoBehaviour
    {
        public YoyoLine yoyoLinePrefab;

        public LineRenderer lineRenderer;

        private HyperBall _hyperBall;

        private void Awake()
        {
            _hyperBall = GetComponent<HyperBall>();
        }

        private void LateUpdate()
        {
            lineRenderer.SetPosition(0, _hyperBall.OwningPlayerRigidbody.position - transform.position);
            lineRenderer.SetPosition(1, Vector3.zero);
        }

        private void OnCollisionEnter(Collision collision)
        {
            var contactPoint = collision.contacts[0];

            var hitPlayerRigidbodyCharacterController = collision.gameObject.GetComponentInChildren<RigidbodyCharacterController>();

            if (hitPlayerRigidbodyCharacterController)
            {
                InstantiateYoyoLine(hitPlayerRigidbodyCharacterController);
            }
            else
            {
                InstantiateYoyoLine(contactPoint);
            }

            Destroy(gameObject);
        }

        private void InstantiateYoyoLine(ContactPoint contactPoint)
        {
            var yoyoLineInstance = Instantiate(yoyoLinePrefab, contactPoint.point, Quaternion.identity);
            yoyoLineInstance.OwningPlayerRigidbody = _hyperBall.OwningPlayerRigidbody;
            yoyoLineInstance.TargetPoint = contactPoint.point;
        }

        private void InstantiateYoyoLine(RigidbodyCharacterController hitPlayerRigidbodyCharacterController)
        {
            var yoyoLineInstance = Instantiate(yoyoLinePrefab, hitPlayerRigidbodyCharacterController.transform.position, Quaternion.identity);
            yoyoLineInstance.OwningPlayerRigidbody = _hyperBall.OwningPlayerRigidbody;
            yoyoLineInstance.TargetCharacterController = hitPlayerRigidbodyCharacterController;
        }
    }
}