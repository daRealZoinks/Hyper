using Unity.Netcode;
using UnityEngine;

namespace Hyper.Player
{
    [RequireComponent(typeof(HyperBall))]
    public class WallHyperBall : NetworkBehaviour
    {
        public NetworkObject wallPrefab;

        private HyperBall _hyperBall;

        private void Awake()
        {
            _hyperBall = GetComponent<HyperBall>();
        }


        private void OnCollisionEnter(Collision collision)
        {
            var contactPoint = collision.contacts[0];

            var playerPos = _hyperBall.OwningPlayerRigidbody.position;
            var toPlayer = playerPos - contactPoint.point;
            toPlayer.y = 0f;

            var rotation = Quaternion.LookRotation(toPlayer.normalized, Vector3.up);

            var wallPrefabInstance = Instantiate(wallPrefab, contactPoint.point, rotation);
            wallPrefabInstance.SpawnWithOwnership(NetworkManager.LocalClientId);

            Destroy(gameObject);
        }
    }
}