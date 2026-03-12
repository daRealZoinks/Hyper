using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GrapplingGunPrototype grapplingGun;
    public bool isGrapplingEnabled;

    private void Update()
    {
        grapplingGun.GrapplingEnabled = isGrapplingEnabled;
    }
}
