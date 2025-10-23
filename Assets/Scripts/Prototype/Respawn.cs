using UnityEngine;

public class Respawn : MonoBehaviour
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

    private void Update()
    {
        if (transform.position.y < -10)
        {
            RespawnAtCheckpoint();
        }
    }

    private void RespawnAtCheckpoint()
    {
        transform.position = checkpoint.transform.position;
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
    }
}
