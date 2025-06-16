using UnityEngine;
using UnityEngine.Events;

public class GroundedManager : MonoBehaviour
{
    public bool IsGrounded { get; private set; }

    public UnityEvent OnLanded;

    private void OnCollisionEnter(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                if (!IsGrounded)
                {
                    IsGrounded = true;
                    OnLanded?.Invoke();
                    break;
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                IsGrounded = true;
                break;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        IsGrounded = false;
    }
}
