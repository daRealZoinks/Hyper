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

        public Button backButton;
        public Button selectButton;

        private readonly Stack<MenuScreen> _menuScreenStack = new();

        private void Awake()
        {
            PushMenuScreen(titleScreen);

            EventSystem.current.GetComponent<InputSystemUIInputModule>().cancel.action.started += _ => PopMenuScreen();

            DeviceManager.Singleton.OnDeviceTypeChanged += UpdateSelectButtonVisibility;
        }

        public void PushMenuScreen(MenuScreen menuScreen)
        {
            if (_menuScreenStack.Count > 0)
            {
                _menuScreenStack.Peek().gameObject.SetActive(false);
            }
            _menuScreenStack.Push(menuScreen);
            menuScreen.gameObject.SetActive(true);

            UpdateBackButtonVisibility();
            UpdateSelectButtonVisibility(DeviceManager.Singleton.currentDeviceType);
        }

        public void PopMenuScreen()
        {
            if (_menuScreenStack.Count > 1)
            {
                var oldMenuScreen = _menuScreenStack.Peek();
                _menuScreenStack.Pop().gameObject.SetActive(false);
                oldMenuScreen.currentSelected = null;
                _menuScreenStack.Peek().gameObject.SetActive(true);
            }

            UpdateBackButtonVisibility();
            UpdateSelectButtonVisibility(DeviceManager.Singleton.currentDeviceType);
        }

        private void UpdateSelectButtonVisibility(DeviceManager.DeviceType deviceType)
        {
            if (!_menuScreenStack.Peek().showButtonPrompts)
            {
                selectButton.gameObject.SetActive(false);
                return;
            }

            var isCurrentDeviceMouse = deviceType == DeviceManager.DeviceType.Mouse;
            var isCurrentDeviceKeyboard = deviceType == DeviceManager.DeviceType.Keyboard;

            selectButton.gameObject.SetActive(!(isCurrentDeviceMouse || isCurrentDeviceKeyboard));
        }

        private void UpdateBackButtonVisibility()
        {
            backButton.gameObject.SetActive(_menuScreenStack.Peek().showButtonPrompts);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}