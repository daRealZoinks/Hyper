using UnityEngine;
using UnityEngine.InputSystem.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuUI;
    public GameObject titleScreenUI;

    public InputSystemUIInputModule inputModule;

    private void OnEnable()
    {
        inputModule.cancel.action.started += (_) => ShowTitleScreen();
    }

    private void OnDisable()
    {
        inputModule.cancel.action.started -= (_) => ShowTitleScreen();
    }

    public void ShowTitleScreen()
    {
        mainMenuUI.SetActive(false);
        titleScreenUI.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
