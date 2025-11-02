using Unity.Netcode;
using UnityEngine;

public class Ball : NetworkBehaviour
{
    public float lifeTime = 15f;

    public delegate void BallCollisionDelegate(ContactPoint contactPoint, RigidbodyCharacterController hitPlayerGameObject);

    public event BallCollisionDelegate OnBallCollision;

    public override void OnNetworkSpawn()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var collisionPoint = collision.contacts[0];

        var playerObject = collision.gameObject.GetComponentInChildren<RigidbodyCharacterController>();

        OnBallCollision?.Invoke(collisionPoint, playerObject);

        Destroy(gameObject);
    }
}
