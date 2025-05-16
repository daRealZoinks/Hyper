using UnityEngine;

public class Respawn : MonoBehaviour
{
    public Transform checkpoint;

    // if you fall under the map respawn at the checkpoint

    private void Update()
    {
        if (transform.position.y < -10)
        {
            RespawnAtCheckpoint();
        }
    }

    private void RespawnAtCheckpoint()
    {
        transform.position = checkpoint.position;
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        Debug.Log("Respawned at checkpoint");
    }
}
