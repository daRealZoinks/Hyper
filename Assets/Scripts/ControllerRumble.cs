using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerRumble : MonoBehaviour
{
    private Gamepad currentGamepad;

    public float strength = 0.5f;
    public float ratio = 0.5f;
    public float duration = 0.1f;

    private void Awake()
    {
        currentGamepad = Gamepad.current;
        currentGamepad.PauseHaptics();

        var lowFrequency = ratio * strength;
        var highFrequency = (1 - ratio) * strength;

        currentGamepad.SetMotorSpeeds(lowFrequency, highFrequency);
    }

    public async void QuickRumble()
    {
        await RumbleAsync(duration);
    }

    public async Task RumbleAsync(float duration)
    {
        if (currentGamepad == null) return;

        currentGamepad.ResumeHaptics();
        await Awaitable.WaitForSecondsAsync(duration);
        currentGamepad.PauseHaptics();
    }
}
