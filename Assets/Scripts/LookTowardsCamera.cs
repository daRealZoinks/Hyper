using Unity.Cinemachine;
using UnityEngine;

public class LookTowardsCamera : MonoBehaviour
{
    public CinemachineCamera cinemachineCamera;

    private void Update()
    {
        if (cinemachineCamera != null)
        {
            var cameraXRotation = cinemachineCamera.transform.rotation.eulerAngles.x;
            transform.rotation = Quaternion.Euler(cameraXRotation, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
    }
}
