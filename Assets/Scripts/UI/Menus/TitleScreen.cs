using UnityEngine;
using UnityEngine.InputSystem.UI;

public class TitleScreen : MonoBehaviour
{
    public GameObject titleScreenUI;
    public GameObject mainMenuUI;

    public InputSystemUIInputModule inputModule;

    private void Awake()
    {
        inputModule.submit.action.started += (_) => ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        titleScreenUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }
}
