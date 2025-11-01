using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.XInput;

public class TitleScreen : MonoBehaviour
{
    public GameObject titleScreenUI;
    public GameObject mainMenuUI;

    public GameObject KeyboardButtonImage;
    public GameObject XboxControllerButtonImage;
    public GameObject PlayStationControllerButtonImage;
    public GameObject SwitchControllerButtonImage;

    public InputSystemUIInputModule inputModule;

    private void Awake()
    {
        inputModule.submit.action.started += (_) => ShowMainMenu();
    }

    private void Update()
    {
        foreach (var device in inputModule.actionsAsset.devices)
        {
            KeyboardButtonImage.SetActive((device is Keyboard keyboard && keyboard == Keyboard.current) || (device is Mouse mouse && mouse == Mouse.current));
            XboxControllerButtonImage.SetActive(device is XInputController xInputController && xInputController == Gamepad.current);
            PlayStationControllerButtonImage.SetActive(device is DualShockGamepad dualShockGamepad && dualShockGamepad == Gamepad.current);
            SwitchControllerButtonImage.SetActive(device is SwitchProControllerHID switchProControllerHID && switchProControllerHID == Gamepad.current);
        }
    }

    public void ShowMainMenu()
    {
        titleScreenUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }
}
