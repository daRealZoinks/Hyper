using Unity.Netcode;
using UnityEngine;

namespace Hyper.Player
{
    public class ThrowBall : NetworkBehaviour
    {
        public HyperBall hyperBallPrefab;
        public Rigidbody playerRigidbody;
        public Transform throwPoint;
        public float throwForce = 20f;

        public float throwInterval = 15f;

        private float _nextThrowTime;

        private void Awake()
        {
            _nextThrowTime = throwInterval;
        }

        private void Update()
        {
            if (_nextThrowTime > 0f)
            {
                _nextThrowTime -= Time.deltaTime;
            }
        }

        public void InstantiateAndThrowBall()
        {
            if (IsOwner)
            {
                if (_nextThrowTime <= 0f)
                {
                    InstantiateAndThrowBallServerRpc(throwPoint.position, throwPoint.forward);
                    LocalInstantiateAndThrowBall(throwPoint.position, throwPoint.forward);

                    _nextThrowTime = throwInterval;
                }
            }
        }

        [Rpc(SendTo.Server)]
        private void InstantiateAndThrowBallServerRpc(Vector3 position, Vector3 direction)
        {
            InstantiateAndThrowBallNotOwnerRpc(position, direction);
        }

        [Rpc(SendTo.NotOwner)]
        private void InstantiateAndThrowBallNotOwnerRpc(Vector3 position, Vector3 direction)
        {
            LocalInstantiateAndThrowBall(position, direction);
        }

        private void LocalInstantiateAndThrowBall(Vector3 position, Vector3 direction)
        {
            var hyperBallInstance = Instantiate(hyperBallPrefab, position, Quaternion.identity);

            hyperBallInstance.OwningPlayerRigidbody = playerRigidbody;

            var spawnedBallRigidbody = hyperBallInstance.GetComponent<Rigidbody>();
            var throwForceVelocity = direction * throwForce + playerRigidbody.linearVelocity;
            spawnedBallRigidbody.AddForce(throwForceVelocity, ForceMode.VelocityChange);
        }
    }
}