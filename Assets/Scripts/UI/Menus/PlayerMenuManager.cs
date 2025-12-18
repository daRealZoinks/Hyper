using UnityEngine;
using UnityEngine.InputSystem;

namespace Hyper.UI.Menus
{
    public class PlayerMenuManager : MonoBehaviour
    {
        public void OnPlayerJoined(PlayerInput playerInput)
        {
            playerInput.transform.SetParent(transform);
        }
    }
}