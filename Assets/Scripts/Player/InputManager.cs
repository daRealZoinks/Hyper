using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(RigidbodyCharacterController))]
public class InputManager : MonoBehaviour
{
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
        if (context.started)
        {
            Debug.Log("Grapple started");
        }

        if (context.canceled)
        {
            Debug.Log("Grapple canceled");
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started)
        {
            return;
        }

        // TODO: put this in another bigger component

        Application.Quit();
    }
}
