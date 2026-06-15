using Hyper.ScriptableObjects;
using Unity.Multiplayer;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayModeConfiguration : MonoBehaviour
{
    public MapInfo mapInfo;

    private void Awake()
    {
        NetworkManager.Singleton.OnServerStarted += () =>
        {
            if (MultiplayerRolesManager.ActiveMultiplayerRoleMask.HasFlag(MultiplayerRoleFlags.Server))
            {
                NetworkManager.Singleton.SceneManager.LoadScene(mapInfo.mapScene.name, LoadSceneMode.Additive);
            }
        };
    }

    private void Start()
    {
        switch (MultiplayerRolesManager.ActiveMultiplayerRoleMask)
        {
            case MultiplayerRoleFlags.Client:
                NetworkManager.Singleton.StartClient();
                break;
            case MultiplayerRoleFlags.Server:
                NetworkManager.Singleton.StartServer();
                break;
            case MultiplayerRoleFlags.ClientAndServer:
                NetworkManager.Singleton.StartHost();
                break;
        }
    }
}
