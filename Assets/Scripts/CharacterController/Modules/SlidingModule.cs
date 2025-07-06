using UnityEngine;

public class SlidingModule : MonoBehaviour
{
    public float gravityScale = 2f;
    public float acceleration = 3f;
    public float topSpeed = 0f;
    public float deceleration = 1f;
    public float slidingCapsuleColliderHeight = 1f;
    public Vector3 slidingCapsuleColliderCenter = new(0f, 0.5f, 0f);
    public Vector3 slidingCameraHolderPosition = new(0f, 0.25f, 0f);
    public float cameraLerpSpeed = 10f;

    public bool IsSliding { get; private set; }

    private Vector3 _cameraHolderOriginalPosition;
    private float _capsuleColliderOriginalHeight;
    private Vector3 _capsuleColliderOriginalCenter;

    private Vector3 targetCameraHolderPosition;

    public Transform _cameraHolder;
    private CapsuleCollider _capsuleCollider;
    private RigidbodyCharacterController _rigidbodyCharacterController;
    private WallJumpModule _wallJumpModule;

    private void Awake()
    {
        _cameraHolderOriginalPosition = _cameraHolder.localPosition;

        _capsuleCollider = GetComponent<CapsuleCollider>();
        _capsuleColliderOriginalHeight = _capsuleCollider.height;
        _capsuleColliderOriginalCenter = _capsuleCollider.center;

        targetCameraHolderPosition = _cameraHolderOriginalPosition;

        _rigidbodyCharacterController = GetComponent<RigidbodyCharacterController>();
        _wallJumpModule = GetComponent<WallJumpModule>();
    }

    private void FixedUpdate()
    {
        if (_rigidbodyCharacterController.CurrentInputPayload.Sliding && !IsSliding)
        {
            StartSliding();
        }
        else
        {
            if (!_rigidbodyCharacterController.CurrentInputPayload.Sliding && IsSliding)
            {
                StopSliding();
            }
        }

        _cameraHolder.localPosition = Vector3.Lerp(_cameraHolder.localPosition, targetCameraHolderPosition, Time.fixedDeltaTime * cameraLerpSpeed);
    }

    private void StartSliding()
    {
        IsSliding = true;
        _wallJumpModule.enabled = false;
        targetCameraHolderPosition = slidingCameraHolderPosition;
        _capsuleCollider.height = slidingCapsuleColliderHeight;
        _capsuleCollider.center = slidingCapsuleColliderCenter;
    }

    private void StopSliding()
    {
        _wallJumpModule.enabled = true;
        targetCameraHolderPosition = _cameraHolderOriginalPosition;
        _capsuleCollider.height = _capsuleColliderOriginalHeight;
        _capsuleCollider.center = _capsuleColliderOriginalCenter;
        IsSliding = false;
    }
}
