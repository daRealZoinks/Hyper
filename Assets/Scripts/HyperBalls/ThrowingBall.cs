using UnityEngine;

public class ThrowingBall : MonoBehaviour
{
    public float operationBase = 1.08f;
    public float influence = 1f;
    public float duration = 1f; // Time to complete the throw arc

    private Transform _originPlayerTransform;
    private Transform _targetPlayerTransform;
    private float _elapsedTime = 0f;
    private Vector3 _startPoint;
    private Vector3 _controlPoint;
    private Vector3 _endPoint;
    private bool _isMoving = false;

    public void Initialize(Transform originPlayerTransform, Transform targetPlayerTransform)
    {
        _originPlayerTransform = originPlayerTransform;
        _targetPlayerTransform = targetPlayerTransform;

        _startPoint = _originPlayerTransform.position;

        var middlePoint = (_startPoint + _targetPlayerTransform.position) * 0.5f;
        var arcHeight = Mathf.Pow(operationBase, Vector3.Distance(_startPoint, _targetPlayerTransform.position) / influence) - 1;
        _controlPoint = middlePoint + Vector3.up * arcHeight;

        _endPoint = _targetPlayerTransform.position;

        _elapsedTime = 0f;
        _isMoving = true;
    }

    private void Update()
    {
        if (_targetPlayerTransform == null || !_isMoving)
        {
            return;
        }

        _elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsedTime / duration);

        // Evaluate the quadratic bezier curve
        Vector3 newPosition = EvaluateBezier(t);
        transform.position = newPosition;

        // Draw the bezier curve for debugging
        Debug.DrawLine(_startPoint, _controlPoint, Color.red);
        Debug.DrawLine(_controlPoint, _endPoint, Color.red);

        if (t >= 1f)
        {
            _isMoving = false;
            Destroy(gameObject);
        }
    }

    private Vector3 EvaluateBezier(float t)
    {
        // Quadratic bezier curve: B(t) = (1-t)²P0 + 2(1-t)tP1 + t²P2
        return Mathf.Pow(1-t, 2) * _startPoint + 2 * (1 - t) * t * _controlPoint + Mathf.Pow(t, 2) * _endPoint;
    }
}