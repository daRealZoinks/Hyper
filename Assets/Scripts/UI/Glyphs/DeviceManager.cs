using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.XInput;

namespace Hyper.UI.Glyphs
{
    public class DeviceManager : MonoBehaviour
    {
        public static DeviceManager Singleton;

        public enum DeviceType
        {
            Mouse,
            Keyboard,
            XboxController,
            PlayStationController,
            SwitchController
        }

        public event Action<DeviceType> OnDeviceTypeChanged;

        private DeviceType currentDeviceType;

        private PlayerInput playerInput;

        private void Awake()
        {
            if (Singleton == null)
            {
                Singleton = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }

            playerInput = EventSystem.current.GetComponent<PlayerInput>();
        }

        private void Update()
        {
            foreach (var device in playerInput.devices)
            {
                DeviceType deviceType;

                switch (device)
                {
                    case Mouse mouse when mouse == Mouse.current:
                        deviceType = DeviceType.Mouse;
                        break;
                    case Keyboard keyboard when keyboard == Keyboard.current:
                        deviceType = DeviceType.Keyboard;
                        break;
                    case XInputController xInputController when xInputController == Gamepad.current:
                        deviceType = DeviceType.XboxController;
                        break;
                    case DualShockGamepad dualShockGamepad when dualShockGamepad == Gamepad.current:
                        deviceType = DeviceType.PlayStationController;
                        break;
                    case SwitchProControllerHID switchProControllerHID when switchProControllerHID == Gamepad.current:
                        deviceType = DeviceType.SwitchController;
                        break;
                    default:
                        continue;
                }

                if (deviceType != currentDeviceType)
                {
                    currentDeviceType = deviceType;
                    OnDeviceTypeChanged?.Invoke(deviceType);
                }
            }
        }
    }
}