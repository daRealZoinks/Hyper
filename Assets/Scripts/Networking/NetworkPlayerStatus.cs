using Unity.Netcode;
using UnityEngine.Events;

public class NetworkPlayerStatus : NetworkBehaviour
{
    public UnityEvent DoOnSpawnIfOwner;
    public UnityEvent DoOnSpawnIfNotOwner;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        (IsOwner ? DoOnSpawnIfOwner : DoOnSpawnIfNotOwner)?.Invoke();
    }
}
