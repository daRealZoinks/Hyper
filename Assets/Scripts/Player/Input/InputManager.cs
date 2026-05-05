using Hyper.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hyper.Player.Input
{
    [RequireComponent(typeof(RigidbodyCharacterController))]
    public class InputManager : MonoBehaviour
    {
        public BallThrower ballThrower;

        public GrapplingGun grapplingGun;

        private RigidbodyCharacterController _rigidbodyCharacterController;

        private void Awake()
        {
            _rigidbodyCharacterController = GetComponent<RigidbodyCharacterController>();

            if (CursorManager.Singleton)
            {
                CursorManager.Singleton.LockAndHideCursor();
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            var moveInput = context.phase switch
            {
                InputActionPhase.Started or InputActionPhase.Performed => context.ReadValue<Vector2>(),
                InputActionPhase.Canceled or InputActionPhase.Waiting or InputActionPhase.Disabled or _ => Vector2.zero,
            };

            _rigidbodyCharacterController.MoveInput = moveInput;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _rigidbodyCharacterController.Jump();
            }
        }

        public void OnSlide(InputAction.CallbackContext context)
        {
            var slide = context.phase switch
            {
                InputActionPhase.Started or InputActionPhase.Performed => true,
                InputActionPhase.Canceled or InputActionPhase.Waiting or InputActionPhase.Disabled or _ => false,
            };

            _rigidbodyCharacterController.Sliding = slide;
        }

        public void OnGrapple(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    grapplingGun.StartGrapple();
                    break;
                case InputActionPhase.Canceled:
                    grapplingGun.StopGrapple();
                    break;
            }
        }

        public void OnThrow(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                ballThrower.ThrowBall();
            }
        }
    }
}