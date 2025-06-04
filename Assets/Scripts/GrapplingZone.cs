using UnityEngine;

public class GrapplingZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var playerManager = other.GetComponent<PlayerManager>();
        if (playerManager)
        {
            playerManager.isGrapplingEnabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var playerManager = other.GetComponent<PlayerManager>();
        if (playerManager)
        {
            playerManager.isGrapplingEnabled = false;
        }
    }
}
