using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Button firstButtonToSelect;

    private void OnEnable()
    {
        firstButtonToSelect.Select();
    }
}
