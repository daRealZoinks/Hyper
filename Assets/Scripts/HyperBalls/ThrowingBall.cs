using System;
using UnityEngine;

public class ThrowingBall : MonoBehaviour
{
    public float arcHeightBase = 1.11f;
    public float arcHeightDistanceDivisor = 1.5f;
    public float travelTimePerDistance = 0.04f;

    public bool shouldSpinHaveRandomRange = true;

    public Transform OriginPlayerTransform { get; private set; }

    public event Action<Rigidbody, Rigidbody, float> OnTargetHit;

    private const float MaxSpinAngleDegrees = 45f;
    private const float SpinAngleDistanceDivisor = 40f;

    private Transform _targetPlayerTransform;

    private float _trajectorDuration;
    private float _arcHeight;
    private float _randomSpinAngle;
    private Vector3 _spinAxis;
    private float _elapsedTime;

    private bool _isInitialized;

    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    public void Initialize(Transform originPlayerTransform, Transform targetPlayerTransform)
    {
        if (originPlayerTransform == null || targetPlayerTransform == null)
        {
            Debug.LogError("ThrowingBall.Initialize: Origin or target transform is null.", this);
            Destroy(gameObject);
            return;
        }

        this.OriginPlayerTransform = originPlayerTransform;
        _targetPlayerTransform = targetPlayerTransform;

        var throwDirection = _targetPlayerTransform.position - this.OriginPlayerTransform.position;
        var throwDistance = throwDirection.magnitude;

        _trajectorDuration = throwDistance * travelTimePerDistance;
        _arcHeight = Mathf.Pow(arcHeightBase, throwDistance / arcHeightDistanceDivisor) - 1f;

        CalculateSpinAxis(throwDirection, throwDistance);

        _elapsedTime = 0f;
        _isInitialized = true;
    }

    private void CalculateSpinAxis(Vector3 throwDirection, float throwDistance)
    {
        var normalizedDirection = throwDirection.normalized;
        var spinAngle = MaxSpinAngleDegrees * throwDistance / SpinAngleDistanceDivisor;

        _randomSpinAngle = UnityEngine.Random.Range(-spinAngle, spinAngle);

        if (!shouldSpinHaveRandomRange)
        {
            if (_randomSpinAngle < 0f)
            {
                _randomSpinAngle = -spinAngle;
            }
            else
            {
                _randomSpinAngle = spinAngle;
            }
        }

        var perpendicularAxis = Vector3.Cross(normalizedDirection, OriginPlayerTransform.right);

        perpendicularAxis = perpendicularAxis.normalized;
        _spinAxis = (Quaternion.AngleAxis(_randomSpinAngle, normalizedDirection) * perpendicularAxis).normalized;
    }

    private void Update()
    {
        if (!_isInitialized || OriginPlayerTransform == null || _targetPlayerTransform == null)
        {
            return;
        }

        _elapsedTime += Time.deltaTime;
        var trajectoryCompletion = Mathf.Clamp01(_elapsedTime / _trajectorDuration);

        var midpoint = (OriginPlayerTransform.position + _targetPlayerTransform.position) * 0.5f;
        var controlPoint = midpoint + _spinAxis * _arcHeight;

        transform.position = EvaluateQuadraticBezier(trajectoryCompletion, OriginPlayerTransform.position, controlPoint, _targetPlayerTransform.position);

        if (trajectoryCompletion >= 1f)
        {
            InvokeOnHitEvent();
            Destroy(gameObject);
        }
    }

    private Vector3 EvaluateQuadraticBezier(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        var oneMintT = 1f - t;
        var oneMintTSq = oneMintT * oneMintT;
        var tSq = t * t;

        return oneMintTSq * p0 + 2f * oneMintT * t * p1 + tSq * p2;
    }

    private void InvokeOnHitEvent()
    {
        var originPlayerRigidbody = OriginPlayerTransform.GetComponent<Rigidbody>();
        var targetPlayerRigidbody = _targetPlayerTransform.GetComponent<Rigidbody>();

        OnTargetHit?.Invoke(originPlayerRigidbody, targetPlayerRigidbody, _randomSpinAngle);
    }
}