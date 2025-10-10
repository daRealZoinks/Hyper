using UnityEngine;

public class RespawnOnCollision : MonoBehaviour
{
    private Checkpoint checkpoint;

    private void Awake()
    {
        checkpoint = FindFirstObjectByType<Checkpoint>();
        if (checkpoint == null)
        {
            Debug.LogError("No Checkpoint found in the scene.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        OldRigidbodyCharacterController rigidbodyCharacterController = collision.gameObject.GetComponent<OldRigidbodyCharacterController>();
        if (rigidbodyCharacterController)
        {
            RespawnAtCheckpoint(rigidbodyCharacterController);
        }
    }

    private void RespawnAtCheckpoint(OldRigidbodyCharacterController rigidbodyCharacterController)
    {
        if (!rigidbodyCharacterController || !checkpoint)
        {
            return;
        }

        rigidbodyCharacterController.transform.position = checkpoint.transform.position;
        rigidbodyCharacterController.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
    }
}
