using Hyper.Player.Input;
using UnityEngine;

namespace Hyper.UI.Menus
{
    public class PauseMenu : MonoBehaviour
    {
        public InputManager inputManager;

        public GameObject pauseMenuUI;

        private bool isPaused = false;

        public void Pause()
        {
            isPaused = !isPaused;
        }

        private void Update()
        {
            inputManager.enabled = !isPaused;
            pauseMenuUI.SetActive(isPaused);
        }
    }
}