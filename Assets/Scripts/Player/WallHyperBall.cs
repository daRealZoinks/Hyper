using UnityEngine;

namespace Hyper.Player
{
    [RequireComponent(typeof(HyperBall))]
    public class WallHyperBall : MonoBehaviour
    {
        public Wall wallPrefab;

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

            InstantiateWallFacingPlayer(contactPoint.point, rotation);

            Destroy(gameObject);
        }

        private void InstantiateWallFacingPlayer(Vector3 position, Quaternion rotation)
        {
            Instantiate(wallPrefab, position, rotation);
        }
    }
}