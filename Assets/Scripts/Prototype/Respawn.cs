using Hyper.Core;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    public float respawnHeight = -40f;

    private PlayerProgress _playerProgress;

    private void Awake()
    {
        _playerProgress = GetComponent<PlayerProgress>();
    }

    private void Update()
    {
        if (transform.position.y < respawnHeight)
        {
            RespawnAtCheckpoint();
        }
    }

    private void RespawnAtCheckpoint()
    {
        transform.position = _playerProgress.lastReachedCheckpoint.Value.transform.position;
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
    }
}
