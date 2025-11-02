using UnityEngine;
using UnityEngine.UI;

public class MultiplayerMenu : MonoBehaviour
{
    public Button firstButtonToSelect;

    private void OnEnable()
    {
        firstButtonToSelect.Select();
    }
}
