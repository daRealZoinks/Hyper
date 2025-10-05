using UnityEngine;
using UnityEngine.Events;

public class GroundCheckModule : MonoBehaviour
{
    [SerializeField]
    private float slopeLimit = 45f;

    public UnityEvent OnLanded;

    public bool IsGrounded { get; private set; }
    public Vector3 GroundNormal { get; private set; }

    private void OnCollisionEnter(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            var angle = Vector3.Angle(contact.normal, Vector3.up);

            if (angle <= slopeLimit)
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

            if (angle <= slopeLimit)
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
