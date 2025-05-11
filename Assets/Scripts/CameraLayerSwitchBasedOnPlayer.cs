using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLayerSwitchBasedOnPlayer : MonoBehaviour
{
    public PlayerInput playerInput;

    public CinemachineCamera cinemachineCamera;
    public CinemachineBrain cinemachineBrain;

    public CinemachineInputAxisController mouseCinemachineInputAxisController;
    public CinemachineInputAxisController controllerCinemachineInputAxisController;

    private void Awake()
    {
        mouseCinemachineInputAxisController.PlayerIndex = playerInput.user.index;
        controllerCinemachineInputAxisController.PlayerIndex = playerInput.user.index;
    }

    private void Start()
    {
        var id = playerInput.user.index;

        var outputChannels = id switch
        {
            0 => OutputChannels.Channel01,
            1 => OutputChannels.Channel02,
            2 => OutputChannels.Channel03,
            3 => OutputChannels.Channel04,
            _ => OutputChannels.Default,
        };

        cinemachineBrain.ChannelMask = outputChannels;
        cinemachineCamera.OutputChannel = outputChannels;
    }

    private void Update()
    {
        mouseCinemachineInputAxisController.enabled = playerInput.currentControlScheme == "Keyboard&Mouse";
        controllerCinemachineInputAxisController.enabled = playerInput.currentControlScheme == "Gamepad";
    }
}
