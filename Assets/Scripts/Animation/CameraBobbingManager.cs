using Unity.Cinemachine;
using UnityEngine;

public class CameraBobbingManager : MonoBehaviour
{
    public new Rigidbody rigidbody;
    public MovementModule movementModule;
    public GroundCheckModule groundCheckModule;
    public WallRunModule wallRunModule;
    public SlidingModule slidingModule;

    private CinemachineBasicMultiChannelPerlin _cinemachineBasicMultiChannelPerlin;

    private void Awake()
    {
        _cinemachineBasicMultiChannelPerlin = GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void FixedUpdate()
    {
        var intensity = 0f;

        if ((groundCheckModule.IsGrounded || wallRunModule.IsWallRunning) && !slidingModule.IsSliding)
        {
            var rigidbodyHorizontalVelocity = new Vector3
            {
                x = rigidbody.linearVelocity.x,
                z = rigidbody.linearVelocity.z
            };

            var speed = rigidbodyHorizontalVelocity.magnitude / movementModule.topSpeed;
            intensity = Mathf.Clamp(speed, 0f, 1f);
        }

        _cinemachineBasicMultiChannelPerlin.AmplitudeGain = intensity;
        _cinemachineBasicMultiChannelPerlin.FrequencyGain = intensity;
    }
}
