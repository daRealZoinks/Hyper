using UnityEngine;
using UnityEngine.UI;

public class SinglePlayerMenu : MonoBehaviour
{
    public Button firstButtonToSelect;

    private void OnEnable()
    {
        firstButtonToSelect.Select();
    }
}
