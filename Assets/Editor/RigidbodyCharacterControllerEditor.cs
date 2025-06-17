using UnityEditor;

[CustomEditor(typeof(RigidbodyCharacterController))]
public class RigidbodyCharacterControllerEditor : Editor
{
    private RigidbodyCharacterController _controller;

    private void OnEnable()
    {
        _controller = (RigidbodyCharacterController)target;
    }
}
