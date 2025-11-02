using Unity.Netcode;
using UnityEngine;

public class ThrowBall : NetworkBehaviour
{
    public NetworkObject throwBall;
    public Rigidbody playerRigidbody;
    public Transform throwPoint;
    public float throwForce = 500f;

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
        var spawnedBall = NetworkManager.SpawnManager.InstantiateAndSpawn(throwBall, position: throwPoint.position, rotation: throwPoint.rotation, ownerClientId: NetworkManager.LocalClientId);

        var ballRigidbody = spawnedBall.GetComponent<Rigidbody>();

        var ball = spawnedBall.GetComponent<Ball>();

        ball.OnBallCollision += (contactPoint, hitPlayerGameObject) =>
        {
            if (hitPlayerGameObject)
            {
                playerRigidbody.position = hitPlayerGameObject.GetComponent<Rigidbody>().position;
            }
            else
            {
                playerRigidbody.position = contactPoint.point + contactPoint.normal;
            }
        };

        var throwForceVelocity = throwPoint.forward * throwForce + playerRigidbody.linearVelocity;
        ballRigidbody.AddForce(throwForceVelocity, ForceMode.VelocityChange);
    }
}
