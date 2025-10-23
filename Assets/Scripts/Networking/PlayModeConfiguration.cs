using Unity.Multiplayer.Playmode;
using Unity.Netcode;
using UnityEngine;

public class PlayModeConfiguration : MonoBehaviour
{
    private void Start()
    {
        foreach (var tag in CurrentPlayer.ReadOnlyTags())
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
