using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(RigidbodyCharacterController))]
public class InputManager : MonoBehaviour
{
    public GrapplingGun grapplingGun;

    private RigidbodyCharacterController _rigidbodyCharacterController;

    private void Awake()
    {
        _rigidbodyCharacterController = GetComponent<RigidbodyCharacterController>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        var moveInput = context.phase switch
        {
            InputActionPhase.Started or InputActionPhase.Performed => context.ReadValue<Vector2>(),
            InputActionPhase.Disabled or InputActionPhase.Waiting or InputActionPhase.Canceled or _ => Vector2.zero,
        };

        _rigidbodyCharacterController.MoveInput = moveInput;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started)
        {
            return;
        }

        _rigidbodyCharacterController.JumpPressed = true;
    }

    public void OnSlide(InputAction.CallbackContext context)
    {
        var slide = context.phase switch
        {
            InputActionPhase.Started or InputActionPhase.Performed => true,
            InputActionPhase.Disabled or InputActionPhase.Waiting or InputActionPhase.Canceled or _ => false,
        };

        _rigidbodyCharacterController.Sliding = slide;
    }

    public void OnGrapple(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Disabled:
                break;
            case InputActionPhase.Waiting:
                break;
            case InputActionPhase.Started:
                grapplingGun.StartGrapple();
                break;
            case InputActionPhase.Performed:
                break;
            case InputActionPhase.Canceled:
                grapplingGun.StopGrapple();
                break;
            default:
                break;
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started)
        {
            return;
        }

        Application.Quit();
    }
}
