using Hyper.Player;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class BallThrower : MonoBehaviour
{
    public float maxDistance = 40f;

    public float cooldown = 15f;
    public float selectionBoxRatio = 0.75f;

    public LayerMask characterLayerMask;
    public RigidbodyCharacterController characterController;
    public ThrowingBall throwingBallPrefab;

    private float _cooldownCounter = 0f;

    private RigidbodyCharacterController _candidate;

    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        _candidate = FindClosestCharacterInSelectionCircle();

        if (_cooldownCounter > 0f)
        {
            _cooldownCounter -= Time.deltaTime;
        }
    }

    private void OnGUI()
    {
        // draw text for the candidate character
        if (_candidate != null)
        {
            var screenPoint = _camera.WorldToScreenPoint(_candidate.transform.position);
            if (screenPoint.z > 0f)
            {
                var label = $"Candidate: {_candidate.name}";

                if (_cooldownCounter > 0f)
                {
                    label += $" (Cooldown: {_cooldownCounter:F1}s)";
                }
                else
                {
                    label += " (Loaded)";
                }

                var labelSize = GUI.skin.label.CalcSize(new GUIContent(label));
                var labelRect = new Rect(screenPoint.x - labelSize.x * 0.5f, Screen.height - screenPoint.y - labelSize.y, labelSize.x, labelSize.y);
                GUI.Label(labelRect, label);
            }
        }
    }

    private RigidbodyCharacterController FindClosestCharacterInSelectionCircle()
    {
        var results = new Collider[10];

        var overlapSphereHitNumber = Physics.OverlapSphereNonAlloc(_camera.transform.position, maxDistance, results, characterLayerMask);

        var colliders = results.Take(overlapSphereHitNumber);

        if (colliders == null || colliders.Count() == 0)
        {
            return null;
        }

        var characters = colliders
            .Where(collider => collider != null)
            .Where(collider =>
            {
                if (Physics.Raycast(_camera.transform.position, collider.transform.position - _camera.transform.position, out var hitInfo, maxDistance))
                {
                    if (collider.gameObject == hitInfo.collider.gameObject)
                    {
                        return true;
                    }
                }

                return false;
            })
            .Select(collider => collider.GetComponent<RigidbodyCharacterController>())
            .Where(rcc => rcc != null)
            .ToList();

        if (characters.Count == 0)
        {
            return null;
        }

        var viewPort = _camera.pixelRect;
        var half = (float)(Mathf.Min(viewPort.width, viewPort.height) * selectionBoxRatio) * 0.5f;

        var screenCenter = new Vector2(viewPort.x + viewPort.width * 0.5f, viewPort.y + viewPort.height * 0.5f);

        RigidbodyCharacterController bestCharacter = null;
        var bestDistanceSquared = float.MaxValue;

        foreach (var character in characters)
        {
            var screenPoint = _camera.WorldToScreenPoint(character.transform.position);
            if (screenPoint.z < 0f)
            {
                continue;
            }

            var distanceFromTheCenter = Vector2.Distance(screenPoint, screenCenter);

            if (distanceFromTheCenter <= half)
            {
                if (distanceFromTheCenter < bestDistanceSquared)
                {
                    bestDistanceSquared = distanceFromTheCenter;
                    bestCharacter = character;
                }
            }
        }

        return bestCharacter;
    }

    public void ThrowBall()
    {
        if (_cooldownCounter <= 0f && _candidate != null)
        {
            ExecuteThrowBall();
            _cooldownCounter = cooldown;
        }
    }

    private void ExecuteThrowBall()
    {
        var throwingBall = Instantiate(throwingBallPrefab);
        throwingBall.Initialize(characterController.transform, _candidate.transform);
    }
}
