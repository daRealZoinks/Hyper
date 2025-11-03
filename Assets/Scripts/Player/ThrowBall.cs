using Unity.Netcode;
using UnityEngine;

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
        var spawnedBall = Instantiate(throwBall, throwPoint.position, throwPoint.rotation);
        spawnedBall.SpawnWithOwnership(NetworkManager.LocalClientId);

        var ballRigidbody = spawnedBall.GetComponent<Rigidbody>();

        var ball = spawnedBall.GetComponent<Ball>();

        ball.OnBallCollision += OnBallCollision;

        var throwForceVelocity = throwPoint.forward * throwForce + playerRigidbody.linearVelocity;
        ballRigidbody.AddForce(throwForceVelocity, ForceMode.VelocityChange);
    }

    private void OnBallCollision(ContactPoint contactPoint, RigidbodyCharacterController hitPlayerGameObject)
    {
        if (hitPlayerGameObject)
        {
            (playerRigidbody.position, hitPlayerGameObject.GetComponent<Rigidbody>().position) = (hitPlayerGameObject.GetComponent<Rigidbody>().position, playerRigidbody.position);
        }
        else
        {
            playerRigidbody.position = contactPoint.point + contactPoint.normal;
        }
    }
}
