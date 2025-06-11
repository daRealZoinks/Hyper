using UnityEditor;

[CustomEditor(typeof(RigidbodyCharacterController))]
public class RigidbodyCharacterControllerEditor : Editor
{
    private RigidbodyCharacterController _controller;

    private void OnEnable()
    {
        _controller = (RigidbodyCharacterController)target;
    }

    //public override void OnInspectorGUI()
    //{
    //    EditorGUILayout.LabelField("Movement", EditorStyles.boldLabel);
    //    _controller.acceleration = EditorGUILayout.FloatField("Acceleration", _controller.acceleration);
    //    _controller.topSpeed = EditorGUILayout.FloatField("Top Speed", _controller.topSpeed);
    //    _controller.deceleration = EditorGUILayout.FloatField("Deceleration", _controller.deceleration);

    //    EditorGUILayout.LabelField("Air movement", EditorStyles.boldLabel);
    //    _controller.jumpHeight = EditorGUILayout.FloatField("Jump Height", _controller.jumpHeight);
    //    _controller.gravityScale = EditorGUILayout.FloatField("Gravity Scale", _controller.gravityScale);
    //    _controller.airControl = EditorGUILayout.FloatField("Air Control", _controller.airControl);
    //    _controller.airBreak = EditorGUILayout.FloatField("Air Break", _controller.airBreak);

    //    EditorGUILayout.LabelField("Wall running", EditorStyles.boldLabel);
    //    _controller.wallRunInitialImpulse = EditorGUILayout.FloatField("Wall Run Initial Impulse", _controller.wallRunInitialImpulse);
    //    _controller.wallCheckDistance = EditorGUILayout.FloatField("Wall Check Distance", _controller.wallCheckDistance);
    //    _controller.wallJumpHeight = EditorGUILayout.FloatField("Wall Jump Height", _controller.wallJumpHeight);
    //    _controller.wallJumpSideForce = EditorGUILayout.FloatField("Wall Jump Side Force", _controller.wallJumpSideForce);
    //    _controller.wallJumpForwardForce = EditorGUILayout.FloatField("Wall Jump Forward Force", _controller.wallJumpForwardForce);

    //    EditorGUILayout.LabelField("References", EditorStyles.boldLabel);
    //    _controller.camera = (Camera)EditorGUILayout.ObjectField("Camera", _controller.camera, typeof(Camera), true);

    //    // Fix for UnityEvent field
    //    var serializedObject = new SerializedObject(_controller);

    //    var onJumpProperty = serializedObject.FindProperty("OnJump");
    //    EditorGUILayout.PropertyField(onJumpProperty, new GUIContent("On Jump"));
    //    serializedObject.ApplyModifiedProperties();

    //    var onLandedProperty = serializedObject.FindProperty("OnLanded");
    //    EditorGUILayout.PropertyField(onLandedProperty, new GUIContent("On Landed"));
    //    serializedObject.ApplyModifiedProperties();

    //    PrefabUtility.RecordPrefabInstancePropertyModifications(_controller);
    //}
}
