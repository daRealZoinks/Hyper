using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button firstButtonToSelect;

    public EventSystem eventSystem;

    private void OnEnable()
    {
        firstButtonToSelect.Select();
    }

    private void OnDisable()
    {
        eventSystem.SetSelectedGameObject(null);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
