using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hyper.Player.Camera
{
    public class CinemachinePlayerCameraAssigning : MonoBehaviour
    {
        public PlayerInput playerInput;

        public CinemachineCamera cinemachineCamera;
        public CinemachineBrain cinemachineBrain;

        public CinemachineInputAxisController mouseCinemachineInputAxisController;
        public CinemachineInputAxisController controllerCinemachineInputAxisController;

        private void Awake()
        {
            if (playerInput.user.valid)
            {
                mouseCinemachineInputAxisController.PlayerIndex = playerInput.user.index;
                controllerCinemachineInputAxisController.PlayerIndex = playerInput.user.index;
            }
        }

        private void Start()
        {
            var outputChannels = playerInput.user.index switch
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
    }
}
