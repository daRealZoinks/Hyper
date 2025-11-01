using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.XInput;

public class DeviceManager : MonoBehaviour
{
    public static DeviceManager Singleton;

    public enum DeviceType
    {
        Keyboard,
        XboxController,
        PlayStationController,
        SwitchController
    }

    public DeviceType CurrentDeviceType { get; set; }

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
            if ((device is Keyboard keyboard && keyboard == Keyboard.current) || (device is Mouse mouse && mouse == Mouse.current))
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
