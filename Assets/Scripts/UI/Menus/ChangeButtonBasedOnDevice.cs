using UnityEngine;

public class ChangeButtonBasedOnDevice : MonoBehaviour
{
    public GameObject KeyboardButtonImage;
    public GameObject XboxControllerButtonImage;
    public GameObject PlayStationControllerButtonImage;
    public GameObject SwitchControllerButtonImage;

    private void Update()
    {
        switch (DeviceManager.Singleton.CurrentDeviceType)
        {
            case DeviceManager.DeviceType.Keyboard:
                KeyboardButtonImage.SetActive(true);
                XboxControllerButtonImage.SetActive(false);
                PlayStationControllerButtonImage.SetActive(false);
                SwitchControllerButtonImage.SetActive(false);
                break;
            case DeviceManager.DeviceType.XboxController:
                XboxControllerButtonImage.SetActive(true);
                KeyboardButtonImage.SetActive(false);
                PlayStationControllerButtonImage.SetActive(false);
                SwitchControllerButtonImage.SetActive(false);
                break;
            case DeviceManager.DeviceType.PlayStationController:
                PlayStationControllerButtonImage.SetActive(true);
                KeyboardButtonImage.SetActive(false);
                XboxControllerButtonImage.SetActive(false);
                SwitchControllerButtonImage.SetActive(false);
                break;
            case DeviceManager.DeviceType.SwitchController:
                SwitchControllerButtonImage.SetActive(true);
                KeyboardButtonImage.SetActive(false);
                XboxControllerButtonImage.SetActive(false);
                PlayStationControllerButtonImage.SetActive(false);
                break;
            default:
                break;
        }
    }
}
