using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.UI;
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

        public DeviceType CurrentDeviceType { get; private set; }

        public InputSystemUIInputModule inputModule;

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
        }

        private void Update()
        {
            foreach (var device in inputModule.actionsAsset.devices)
            {
                if (device is Mouse mouse && mouse == Mouse.current)
                {
                    CurrentDeviceType = DeviceType.Mouse;
                }
                else if (device is Keyboard keyboard && keyboard == Keyboard.current)
                {
                    CurrentDeviceType = DeviceType.Keyboard;
                }
                else if (device is XInputController xInputController && xInputController == Gamepad.current)
                {
                    CurrentDeviceType = DeviceType.XboxController;
                }
                else if (device is DualShockGamepad dualShockGamepad && dualShockGamepad == Gamepad.current)
                {
                    CurrentDeviceType = DeviceType.PlayStationController;
                }
                else if (device is SwitchProControllerHID switchProControllerHID && switchProControllerHID == Gamepad.current)
                {
                    CurrentDeviceType = DeviceType.SwitchController;
                }
            }
        }
    }
}