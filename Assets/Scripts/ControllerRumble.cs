using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerRumble : MonoBehaviour
{
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public void Rumble()
    {
        Rumble(0.4f, 0.1f);
    }

    public void Rumble(float strength, float duration)
    {
        if (playerInput.currentControlScheme != "Gamepad")
        {
            return;
        }

        var devices = playerInput.devices;

        foreach (var device in devices)
        {
            if (device is Gamepad gamepad)
            {
                if (gamepad == Gamepad.current)
                {
                    // Rumble the gamepad
                    gamepad.SetMotorSpeeds(2 / 3f * strength, 1 / 3f * strength);
                    Invoke(nameof(StopRumble), duration);
                }
            }
        }
    }

    private void StopRumble()
    {
        Gamepad.current.SetMotorSpeeds(0, 0);
    }
}
