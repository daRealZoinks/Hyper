using Unity.Multiplayer.PlayMode;
using Unity.Netcode;
using UnityEngine;

public class PlayModeConfiguration : MonoBehaviour
{
    private void Start()
    {
        foreach (var tag in CurrentPlayer.Tags)
        {
            if (tag == "Host")
            {
                NetworkManager.Singleton.StartHost();
            }

            if (tag == "Client")
            {
                NetworkManager.Singleton.StartClient();
            }
        }
    }
}
