using UnityEngine;
using UnityEngine.InputSystem.Users;

public class CharacterSelectMenu : MonoBehaviour
{
    public InputUser User
    {
        get
        {
            return _user;
        }
        set
        {
            _user = value;
            startText.SetActive(false);
        }
    }

    public GameObject startText;
    private InputUser _user;
}
