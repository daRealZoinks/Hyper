using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerRumble : MonoBehaviour
{
    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
    }

    public void LandingRumble()
    {
        _ = RumbleAsync(0.25f, 0.25f, 0.1f);
    }

    public async Task RumbleAsync(float lowFrequency, float highFrequency, float duration)
    {
        foreach (var device in _playerInput.devices)
        {
            if (device is Gamepad gamepad && gamepad == Gamepad.current)
            {
                gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
                await Awaitable.WaitForSecondsAsync(duration);
                gamepad.ResetHaptics();
                return;
            }
        }
    }
}
