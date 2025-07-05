using UnityEditor;

[CustomEditor(typeof(InputManager))]
public class InputManagerEditor : Editor
{
    private InputManager _inputManager;
  
    private void OnEnable()
    {
        _inputManager = (InputManager)target;
    }

    public override void OnInspectorGUI() { }
}