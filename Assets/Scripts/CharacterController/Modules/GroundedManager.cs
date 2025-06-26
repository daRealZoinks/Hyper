using UnityEngine;
using UnityEngine.Events;

public class GroundedManager : MonoBehaviour
{
    public float SlopeLimit = 45f;
    public bool IsGrounded { get; private set; }

    public Vector3 GroundNormal { get; private set; }

    public UnityEvent OnLanded;

    private void OnCollisionEnter(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            var angle = Vector3.Angle(contact.normal, Vector3.up);

            if (angle <= SlopeLimit)
            {
                if (!IsGrounded)
                {
                    IsGrounded = true;
                    GroundNormal = contact.normal;
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
            var angle = Vector3.Angle(contact.normal, Vector3.up);

            if (angle <= SlopeLimit)
            {
                IsGrounded = true;
                GroundNormal = contact.normal;
                break;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        IsGrounded = false;
    }
}
