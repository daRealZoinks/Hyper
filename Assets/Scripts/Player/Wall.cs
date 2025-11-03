using Unity.Netcode;
using UnityEngine;

public class Wall : NetworkBehaviour
{
    public float lifeTime = 15f;
    public float timeToRaise = 0.5f;

    private float _raiseTimer;
    private bool _isRaising;

    public override void OnNetworkSpawn()
    {
        Destroy(gameObject, lifeTime);

        transform.localScale = Vector3.one + Vector3.down;

        if (timeToRaise <= 0f)
        {
            transform.localScale = Vector3.one;
            _isRaising = false;
        }
        else
        {
            _raiseTimer = 0f;
            _isRaising = true;
        }
    }

    private void Update()
    {
        if (_isRaising)
        {
            _raiseTimer += Time.deltaTime;
            var t = Mathf.Clamp01(_raiseTimer / timeToRaise);

            var current = transform.localScale;
            current.y = Mathf.Lerp(0f, 1f, t);
            transform.localScale = current;

            if (t >= 1f)
            {
                _isRaising = false;
            }
        }
    }
}
