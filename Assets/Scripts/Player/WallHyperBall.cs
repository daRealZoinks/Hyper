using Unity.Netcode;
using UnityEngine;

namespace Hyper.Player
{
    [RequireComponent(typeof(HyperBall))]
    public class WallHyperBall : NetworkBehaviour
    {
        public Wall wallPrefab;

        private HyperBall _hyperBall;

        private void Awake()
        {
            _hyperBall = GetComponent<HyperBall>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_hyperBall.OwningPlayerRigidbody.GetComponentInParent<NetworkObject>().IsOwner)
            {
                var contactPoint = collision.contacts[0];

                var playerPos = _hyperBall.OwningPlayerRigidbody.position;
                var toPlayer = playerPos - contactPoint.point;
                toPlayer.y = 0f;

                var rotation = Quaternion.LookRotation(toPlayer.normalized, Vector3.up);

                InstantiateWallFacingPlayerServerRpc(contactPoint.point, rotation);
                InstantiateWallFacingPlayer(contactPoint.point, rotation);
            }

            Destroy(gameObject);
        }

        [Rpc(SendTo.Server)]
        private void InstantiateWallFacingPlayerServerRpc(Vector3 position, Quaternion rotation)
        {
            InstantiateWallFacingPlayerNotOwnerRpc(position, rotation);
        }

        [Rpc(SendTo.NotOwner)]
        private void InstantiateWallFacingPlayerNotOwnerRpc(Vector3 position, Quaternion rotation)
        {
            InstantiateWallFacingPlayer(position, rotation);
        }

        private void InstantiateWallFacingPlayer(Vector3 position, Quaternion rotation)
        {
            Instantiate(wallPrefab, position, rotation);
        }
    }
}