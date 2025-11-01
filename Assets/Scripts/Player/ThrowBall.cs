using Unity.Netcode;
using UnityEngine;

public class ThrowBall : NetworkBehaviour
{
    public NetworkObject throwBall;
    public Rigidbody playerRigidbody;
    public Transform throwPoint;
    public float throwForce = 500f;

    public void InstantiateAndThrowBall()
    {
        var thrownBall = NetworkManager.SpawnManager.InstantiateAndSpawn(throwBall, position: throwPoint.position, rotation: throwPoint.rotation, ownerClientId: NetworkManager.LocalClientId);

        var ballRigidbody = thrownBall.GetComponent<Rigidbody>();

        var throwForceVelocity = throwPoint.forward * throwForce + playerRigidbody.linearVelocity;

        ballRigidbody.AddForce(throwForceVelocity, ForceMode.VelocityChange);
    }
}
