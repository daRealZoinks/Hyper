using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerRumble : MonoBehaviour
{
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public async void QuickRumble()
    {
        await RumbleAsync(0.4f, 0.1f);
    }

    public async Task RumbleAsync(float strength, float duration)
    {
        var isController = playerInput.currentControlScheme != "Xbox" || playerInput.currentControlScheme != "PlayStation";

        if (!isController)
        {
            return;
        }

        var devices = playerInput.devices;

        foreach (var device in devices)
        {
            if (device is not Gamepad gamepad) continue;

            if (gamepad != Gamepad.current) continue;


            var lowFrequency = 1f / 3f * strength;
            var highFrequency = 2f / 3f * strength;

            gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
            await Awaitable.WaitForSecondsAsync(duration);
            StopRumble();
            return;
        }
    }

    private void StopRumble()
    {
        Gamepad.current.SetMotorSpeeds(0, 0);
    }
}
