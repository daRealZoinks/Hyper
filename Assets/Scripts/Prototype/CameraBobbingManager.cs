using Hyper.Player;
using Unity.Cinemachine;
using UnityEngine;

public class CameraBobbingManager : MonoBehaviour
{
    public RigidbodyCharacterController rigidbodyCharacterController;

    private CinemachineBasicMultiChannelPerlin _cinemachineBasicMultiChannelPerlin;

    private void Awake()
    {
        _cinemachineBasicMultiChannelPerlin = GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void FixedUpdate()
    {
        var intensity = 0f;

        if ((rigidbodyCharacterController.IsGrounded || rigidbodyCharacterController.IsWallRunning) && !rigidbodyCharacterController.IsSliding)
        {
            var rigidbody = rigidbodyCharacterController.GetComponent<Rigidbody>();

            var rigidbodyHorizontalVelocity = new Vector3
            {
                x = rigidbody.linearVelocity.x,
                z = rigidbody.linearVelocity.z
            };

            var speed = rigidbodyHorizontalVelocity.magnitude / rigidbodyCharacterController.TopSpeed;
            intensity = Mathf.Clamp(speed, 0f, 1f);
        }

        _cinemachineBasicMultiChannelPerlin.AmplitudeGain = intensity;
        _cinemachineBasicMultiChannelPerlin.FrequencyGain = intensity;
    }
}
