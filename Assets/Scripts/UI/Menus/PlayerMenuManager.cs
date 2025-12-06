using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMenuManager : MonoBehaviour
{
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        playerInput.transform.SetParent(transform);
    }
}
