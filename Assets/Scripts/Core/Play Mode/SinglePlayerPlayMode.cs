using Unity.Netcode;
using UnityEngine;

namespace Hyper.Core.PlayMode
{
    [RequireComponent(typeof(NetworkManager))]
    public class SinglePlayerPlayMode : MonoBehaviour
    {
        public NetworkObject playerNetworkObject;

        private NetworkManager networkManager;

        private void Awake()
        {
            networkManager = GetComponent<NetworkManager>();
        }

        private void Start()
        {
            Setup();
        }

        private void Setup()
        {
            networkManager.StartHost();
            networkManager.SpawnManager.InstantiateAndSpawn(playerNetworkObject);
        }
    }
}