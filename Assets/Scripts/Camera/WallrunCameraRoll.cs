using Hyper.Player;
using UnityEngine;

namespace Hyper.Camera
{
    [RequireComponent(typeof(CinemachinePanTiltRoll))]
    public class WallrunCameraRoll : MonoBehaviour
    {
        public RigidbodyCharacterController rigidbodyCharacterController;
        public int rollSpeed = 5;

        private CinemachinePanTiltRoll cinemachinePanTiltRoll;

        private float angle = 0;

        private void Awake()
        {
            cinemachinePanTiltRoll = GetComponent<CinemachinePanTiltRoll>();
        }

        private void Update()
        {
            if (rigidbodyCharacterController.IsWallRunning)
            {
                if (rigidbodyCharacterController.IsWallRunningOnRightWall)
                {
                    angle = cinemachinePanTiltRoll.RollAxis.Range.y;
                }

                if (rigidbodyCharacterController.IsWallRunningOnLeftWall)
                {
                    angle = cinemachinePanTiltRoll.RollAxis.Range.x;
                }
            }
            else
            {
                angle = 0;
            }

            cinemachinePanTiltRoll.RollAxis.Value = Mathf.Lerp(cinemachinePanTiltRoll.RollAxis.Value, angle, Time.deltaTime * rollSpeed);
        }
    }
}