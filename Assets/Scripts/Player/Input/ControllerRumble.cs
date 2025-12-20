using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hyper.Player.Input
{
    public class ControllerRumble : MonoBehaviour
    {
        private PlayerInput _playerInput;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        public void LandingRumble(float landingVerticalVelocity)
        {
            var landingIntensity = Mathf.Min(landingVerticalVelocity, 100) / 100;

            _ = RumbleAsync(landingIntensity, landingIntensity, 0.1f);
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
}