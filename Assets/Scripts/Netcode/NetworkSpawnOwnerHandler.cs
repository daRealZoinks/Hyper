using Unity.Netcode;
using UnityEngine.Events;

namespace Hyper.Netcode
{
    public class NetworkSpawnOwnerHandler : NetworkBehaviour
    {
        public UnityEvent OnNetworkSpawnIfOwner;
        public UnityEvent OnNetworkSpawnIfNotOwner;

        public override void OnNetworkSpawn()
        {
            (IsOwner ? OnNetworkSpawnIfOwner : OnNetworkSpawnIfNotOwner)?.Invoke();
        }
    }
}