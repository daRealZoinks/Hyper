using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GrapplingGun grapplingGun;
    public bool isGrapplingEnabled;

    private void Update()
    {
        grapplingGun.GrapplingEnabled = isGrapplingEnabled;
    }
}
