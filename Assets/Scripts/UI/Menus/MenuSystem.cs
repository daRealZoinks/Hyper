using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Hyper.UI.Menus
{
    [RequireComponent(typeof(GameSettingsManager))]
    public class MenuSystem : MonoBehaviour
    {
        public MenuScreen titleScreen;
        public MenuScreen singlePlayerCharacterMenuScreen;

        public Button backButton;
        public Button selectButton;

        private readonly Stack<MenuScreen> menuScreenStack = new();
        private GameSettingsManager _gameSettingsManager;

        private void Awake()
        {
            PushMenuScreen(titleScreen);

            _gameSettingsManager = GetComponent<GameSettingsManager>();

            EventSystem.current.GetComponent<InputSystemUIInputModule>().cancel.action.started += _ => PopMenuScreen();

            DeviceManager.Singleton.OnDeviceTypeChanged += UpdateSelectButtonVisibility;
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
            UpdateSelectButtonVisibility(DeviceManager.Singleton.currentDeviceType);
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
            UpdateSelectButtonVisibility(DeviceManager.Singleton.currentDeviceType);
        }

        private void UpdateSelectButtonVisibility(DeviceManager.DeviceType deviceType)
        {
            if (selectButton != null)
            {
                var isCurrentDeviceMouse = deviceType == DeviceManager.DeviceType.Mouse;
                var isCurrentDeviceKeyboard = deviceType == DeviceManager.DeviceType.Keyboard;

                if (isCurrentDeviceMouse || isCurrentDeviceKeyboard)
                {
                    selectButton.gameObject.SetActive(false);
                }
                else
                {
                    selectButton.gameObject.SetActive(menuScreenStack.Count > 1);
                }
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