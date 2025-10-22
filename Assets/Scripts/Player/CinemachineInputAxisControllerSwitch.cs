using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CinemachineInputAxisControllerSwitch : MonoBehaviour
{
    public PlayerInput playerInput;

    public CinemachineInputAxisController mouseCinemachineInputAxisController;
    public CinemachineInputAxisController controllerCinemachineInputAxisController;

    private void Update()
    {
        foreach (var device in playerInput.devices)
        {
            if (device is Mouse)
            {
                SetToMouse();
                return;
            }

            if (device is Gamepad)
            {
                SetToController();
                return;
            }
        }
    }

    public void SetToMouse()
    {
        mouseCinemachineInputAxisController.enabled = true;
        controllerCinemachineInputAxisController.enabled = false;
    }

    public void SetToController()
    {
        mouseCinemachineInputAxisController.enabled = false;
        controllerCinemachineInputAxisController.enabled = true;
    }
}
