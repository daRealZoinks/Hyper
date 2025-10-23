using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnPlayers : MonoBehaviour
{
    public List<Transform> spawnPoints;

    public void OnPlayerJoined(PlayerInput obj)
    {
        obj.gameObject.GetComponent<Rigidbody>().position = spawnPoints[obj.user.index].position;
    }
}
