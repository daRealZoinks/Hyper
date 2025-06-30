using Unity.Cinemachine;
using UnityEngine;

public class CameraBobbingManager : MonoBehaviour
{
    public new Rigidbody rigidbody;
    public MovementManager movementManager;
    public GroundCheckModule groundedManager;
    public WallRunModule wallRunModule;
    public SlidingManager slidingManager;

    private CinemachineBasicMultiChannelPerlin _cinemachineBasicMultiChannelPerlin;

    private void Awake()
    {
        _cinemachineBasicMultiChannelPerlin = GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void FixedUpdate()
    {
        var intensity = 0f;

        if ((groundedManager.IsGrounded || wallRunModule.IsWallRunning) && !slidingManager.IsSliding)
        {
            var rigidbodyHorizontalVelocity = new Vector3
            {
                x = rigidbody.linearVelocity.x,
                z = rigidbody.linearVelocity.z
            };

            var speed = rigidbodyHorizontalVelocity.magnitude / movementManager.topSpeed;
            intensity = Mathf.Clamp(speed, 0f, 1f);
        }

        _cinemachineBasicMultiChannelPerlin.AmplitudeGain = intensity;
        _cinemachineBasicMultiChannelPerlin.FrequencyGain = intensity;
    }
}
