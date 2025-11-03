using Unity.Netcode;
using UnityEngine;

namespace Hyper.Player
{
    public class ThrowBall : NetworkBehaviour
    {
        public NetworkObject throwBall;
        public Rigidbody playerRigidbody;
        public Transform throwPoint;
        public float throwForce = 20f;

        public float throwInterval = 15f;

        private float _nextThrowTime;

        public override void OnNetworkSpawn()
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
            if (_nextThrowTime <= 0f)
            {
                ExecuteInstantiateAndThrowBall();
                _nextThrowTime = throwInterval;
            }
        }

        private void ExecuteInstantiateAndThrowBall()
        {
            var spawnedBallNetworkObject = Instantiate(throwBall, throwPoint.position, throwPoint.rotation);
            spawnedBallNetworkObject.SpawnWithOwnership(NetworkManager.LocalClientId);

            var spawnedBall = spawnedBallNetworkObject.GetComponent<HyperBall>();
            spawnedBall.OwningPlayerRigidbody = playerRigidbody;

            var spawnedBallRigidbody = spawnedBallNetworkObject.GetComponent<Rigidbody>();
            var throwForceVelocity = throwPoint.forward * throwForce + playerRigidbody.linearVelocity;
            spawnedBallRigidbody.AddForce(throwForceVelocity, ForceMode.VelocityChange);
        }
    }
}