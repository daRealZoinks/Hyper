using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Hyper.UI.Menus
{
    public class MenuSystem : MonoBehaviour
    {
        private readonly Stack<MenuScreen> menuScreenStack = new();

        public MenuScreen titleScreen;

        public Button backButton;
        public Button selectButton;

        private void Awake()
        {
            PushMenuScreen(titleScreen);

            EventSystem.current.GetComponent<InputSystemUIInputModule>().cancel.action.started += _ => PopMenuScreen();
        }

        private void Update()
        {
            UpdateSelectButtonVisibility();
        }

        public void PushMenuScreen(MenuScreen menuScreen)
        {
            if (menuScreenStack.Count > 0)
            {
                menuScreenStack.Peek().gameObject.SetActive(false);
            }
            menuScreenStack.Push(menuScreen);
            menuScreen.gameObject.SetActive(true);

            UpdateBackButtonVisibility();
        }

        public void PopMenuScreen()
        {
            if (menuScreenStack.Count > 1)
            {
                var oldMenuScreen = menuScreenStack.Peek();
                menuScreenStack.Pop().gameObject.SetActive(false);
                oldMenuScreen.currentSelected = null;
                menuScreenStack.Peek().gameObject.SetActive(true);
            }

            UpdateBackButtonVisibility();
        }

        private void UpdateSelectButtonVisibility()
        {
            if (selectButton != null)
            {
                selectButton.gameObject.SetActive(EventSystem.current.currentSelectedGameObject && menuScreenStack.Count > 1 &&
                    DeviceManager.Singleton.currentDeviceType != DeviceManager.DeviceType.Mouse &&
                    DeviceManager.Singleton.currentDeviceType != DeviceManager.DeviceType.Keyboard);
            }
        }

        private void UpdateBackButtonVisibility()
        {
            if (backButton != null)
            {
                backButton.gameObject.SetActive(menuScreenStack.Count > 1);
            }
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}