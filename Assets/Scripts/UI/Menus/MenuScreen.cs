using Hyper.UI.Glyphs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Hyper.UI.Menus
{
    public class MenuScreen : MonoBehaviour
    {
        public List<Button> buttons;

        public Button firstButtonToSelect;

        private Button _lastButtonSelected;

        private InputSystemUIInputModule _inputSystemUIInputModule;

        private void Awake()
        {
            var eventSystem = EventSystem.current;

            _inputSystemUIInputModule = eventSystem.GetComponent<InputSystemUIInputModule>();

            _inputSystemUIInputModule.move.action.performed += (_) =>
            {
                if (eventSystem.currentSelectedGameObject == null)
                {
                    (_lastButtonSelected ? _lastButtonSelected : firstButtonToSelect).Select();
                }

                if (eventSystem && eventSystem.currentSelectedGameObject)
                {
                    var button = eventSystem.currentSelectedGameObject.GetComponent<Button>();

                    if (buttons.Contains(button))
                    {
                        _lastButtonSelected = eventSystem.currentSelectedGameObject.GetComponent<Button>();
                    }
                }
            };
        }

        private void OnEnable()
        {
            if (CursorManager.Singleton)
            {
                CursorManager.Singleton.UnlockAndShowCursor();
            }

            if (DeviceManager.Singleton && DeviceManager.Singleton.CurrentDeviceType != DeviceManager.DeviceType.Mouse)
            {
                (_lastButtonSelected ? _lastButtonSelected : firstButtonToSelect).Select();
            }
        }

        private void OnDisable()
        {
            if (CursorManager.Singleton)
            {
                CursorManager.Singleton.LockAndHideCursor();
            }
        }
    }
}