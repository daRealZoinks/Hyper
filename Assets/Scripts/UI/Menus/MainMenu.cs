using UnityEngine;
using UnityEngine.InputSystem.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuUI;
    public GameObject titleScreenUI;

    public InputSystemUIInputModule inputModule;

    private void Awake()
    {
        inputModule.cancel.action.started += (_) => ShowTitleScreen();
    }

    public void ShowTitleScreen()
    {
        mainMenuUI.SetActive(false);
        titleScreenUI.SetActive(true);
    }
}
