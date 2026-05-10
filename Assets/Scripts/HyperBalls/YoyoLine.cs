using Hyper.Player;
using System.Threading.Tasks;
using UnityEngine;

namespace Hyper.HyperBalls
{
    public class YoyoLine : MonoBehaviour
    {
        public float speed = 25f;
        public float rangeToDisengage = 3f;
        public float maxLifetime = 10f;

        public float transitionDuration = 0.5f;

        public float puttingPlayerInFrontOfTargetDuration = 0.2f;

        public LineRenderer lineRenderer;

        public Rigidbody OwnerRigidbody
        {
            private get
            {
                return _ownerRigidbody;
            }
            set
            {
                _ownerRigidbody = value;
                ownerRigidbodyCharacterController = _ownerRigidbody.GetComponent<RigidbodyCharacterController>();
                StartGrappling();
            }
        }
        public Rigidbody TargetRigidbody
        {
            private get
            {
                return _targetRigidbody;
            }
            set
            {
                _targetRigidbody = value;
                targetRigidbodyCharacterController = _targetRigidbody.GetComponent<RigidbodyCharacterController>();
                StartGrappling();
            }
        }

        private float _transitionElapsedTime;
        private bool _disengaging;

        private RigidbodyCharacterController ownerRigidbodyCharacterController;
        private RigidbodyCharacterController targetRigidbodyCharacterController;

        private Rigidbody _ownerRigidbody;
        private Rigidbody _targetRigidbody;

        private void Awake()
        {
            Destroy(gameObject, maxLifetime);
        }

        private void LateUpdate()
        {
            if (TargetRigidbody)
            {
                lineRenderer.SetPosition(0, TargetRigidbody.position - transform.position);
                lineRenderer.SetPosition(1, OwnerRigidbody.position - transform.position);
            }
        }

        private void FixedUpdate()
        {
            if (OwnerRigidbody && TargetRigidbody && !_disengaging)
            {
                var toTarget = TargetRigidbody.position - OwnerRigidbody.position;

                if (_transitionElapsedTime < transitionDuration)
                {
                    _transitionElapsedTime += Time.fixedDeltaTime;
                }

                var transitionProgress = Mathf.Clamp01(_transitionElapsedTime / transitionDuration);

                if (toTarget.magnitude > rangeToDisengage)
                {
                    OwnerRigidbody.linearVelocity = Vector3.Lerp(OwnerRigidbody.linearVelocity, toTarget.normalized * speed, transitionProgress);
                    TargetRigidbody.linearVelocity = Vector3.Lerp(TargetRigidbody.linearVelocity, -toTarget.normalized * speed, transitionProgress);
                }
                else
                {
                    Disengage();
                }
            }
        }

        private void OnDestroy()
        {
            ownerRigidbodyCharacterController.useGravity = true;
            targetRigidbodyCharacterController.useGravity = true;
        }

        private async void Disengage()
        {
            _disengaging = true;

            lineRenderer.enabled = false;

            var ownerInitialPosition = OwnerRigidbody.position;
            var targetInitialPosition = TargetRigidbody.position;

            var finalOwnerPosition = targetInitialPosition - (ownerInitialPosition - targetInitialPosition).normalized * 2;

            var puttingPlayerInFrontOfTargetElapsedTime = 0f;

            var ownerCapsuleCollider = ownerRigidbodyCharacterController.GetComponent<CapsuleCollider>();
            ownerCapsuleCollider.enabled = false;

            if (!Physics.Raycast(targetInitialPosition, (ownerInitialPosition - targetInitialPosition).normalized, Vector3.Distance(targetInitialPosition, ownerInitialPosition)))
            {
                while (puttingPlayerInFrontOfTargetElapsedTime < puttingPlayerInFrontOfTargetDuration)
                {
                    puttingPlayerInFrontOfTargetElapsedTime += Time.deltaTime;
                    var t = Mathf.Clamp01(puttingPlayerInFrontOfTargetElapsedTime / puttingPlayerInFrontOfTargetDuration);
                    OwnerRigidbody.position = Vector3.Lerp(ownerInitialPosition, finalOwnerPosition, t);
                    await Task.Yield();
                }
            }

            ownerCapsuleCollider.enabled = true;

            Destroy(gameObject);
        }

        private void StartGrappling()
        {
            if (ownerRigidbodyCharacterController && targetRigidbodyCharacterController)
            {
                ownerRigidbodyCharacterController.useGravity = false;
                targetRigidbodyCharacterController.useGravity = false;
            }
        }
    }
}