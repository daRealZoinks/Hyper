using UnityEngine;

namespace Hyper.UI.Glyphs
{
    public class ChangeButtonBasedOnDevice : MonoBehaviour
    {
        public GameObject KeyboardButtonImage;
        public GameObject XboxControllerButtonImage;
        public GameObject PlayStationControllerButtonImage;
        public GameObject SwitchControllerButtonImage;

        private void Awake()
        {
            DeviceManager.Singleton.OnDeviceTypeChanged += OnDeviceTypeChanged;
        }

        private void OnDeviceTypeChanged(DeviceManager.DeviceType type)
        {
            var enableKeyboardPrompts = false;
            var enableXboxPrompts = false;
            var enablePlaystationPrompts = false;
            var enableSwitchPrompts = false;

            switch (type)
            {
                case DeviceManager.DeviceType.Mouse or DeviceManager.DeviceType.Keyboard:
                    enableKeyboardPrompts = true;
                    break;
                case DeviceManager.DeviceType.XboxController:
                    enableXboxPrompts = true;
                    break;
                case DeviceManager.DeviceType.PlayStationController:
                    enablePlaystationPrompts = true;
                    break;
                case DeviceManager.DeviceType.SwitchController:
                    enableSwitchPrompts = true;
                    break;
                default:
                    break;
            }

            KeyboardButtonImage.SetActive(enableKeyboardPrompts);
            XboxControllerButtonImage.SetActive(enableXboxPrompts);
            PlayStationControllerButtonImage.SetActive(enablePlaystationPrompts);
            SwitchControllerButtonImage.SetActive(enableSwitchPrompts);
        }
    }
}