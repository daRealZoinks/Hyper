using TMPro;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    TextMeshProUGUI speedText;

    public RigidbodyCharacterController characterController;

    new Rigidbody rigidbody;

    private void Awake()
    {
        speedText = GetComponent<TextMeshProUGUI>();
        rigidbody = characterController.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        var horizontalVelocity = new Vector3
        {
            x = rigidbody.linearVelocity.x,
            z = rigidbody.linearVelocity.z
        };

        float speed = horizontalVelocity.magnitude;
        int speedInt = (int)(speed * 100);
        float speedFloat = (float)speedInt / 100;

        speedText.text = speedFloat.ToString();
    }
}
