using UnityEngine;

public class ThrowingBall : MonoBehaviour
{
    public float operationBase = 1.11f;
    public float archByDistance = 1.5f;
    public float ballTravelDurationBasedOnDistance = 0.04f;

    private Transform _originPlayerTransform;
    private Transform _targetPlayerTransform;

    private float _duration;
    private float _arcHeight;

    private float _elapsedTime;
    private Vector3 _controlPoint;

    public void Initialize(Transform originPlayerTransform, Transform targetPlayerTransform)
    {
        _originPlayerTransform = originPlayerTransform;
        _targetPlayerTransform = targetPlayerTransform;

        _duration = Vector3.Distance(_originPlayerTransform.position, _targetPlayerTransform.position) * ballTravelDurationBasedOnDistance;

        _arcHeight = Mathf.Pow(operationBase, Vector3.Distance(_originPlayerTransform.position, _targetPlayerTransform.position) / archByDistance) - 1;

        _elapsedTime = 0f;
    }

    private void Update()
    {
        if (_targetPlayerTransform == null)
        {
            return;
        }

        var middlePoint = (_originPlayerTransform.position + _targetPlayerTransform.position) * 0.5f;

        _controlPoint = middlePoint + Vector3.up * _arcHeight;

        _elapsedTime += Time.deltaTime;
        var t = Mathf.Clamp01(_elapsedTime / _duration);

        transform.position = EvaluateBezier(t, _originPlayerTransform.position, _controlPoint, _targetPlayerTransform.position);

        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }

    private Vector3 EvaluateBezier(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        return Mathf.Pow(1 - t, 2) * p0 + 2 * (1 - t) * t * p1 + Mathf.Pow(t, 2) * p2;
    }
}