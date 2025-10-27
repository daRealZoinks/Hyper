using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class JoinPlayerLocal : MonoBehaviour
{
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        var networkObject = playerInput.GetComponentInParent<NetworkObject>();

        if (!networkObject.IsSpawned)
        {
            networkObject.Spawn();
        }
    }
}
