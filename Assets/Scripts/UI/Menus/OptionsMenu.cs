using UnityEngine;
using UnityEngine.InputSystem.UI;

public class OptionsMenu : MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject mainMenuUI;

    public InputSystemUIInputModule inputModule;

    private void OnEnable()
    {
        inputModule.cancel.action.started += (_) => ShowMainMenu();
    }

    private void OnDisable()
    {
        inputModule.cancel.action.started -= (_) => ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        optionsMenu.SetActive(false);
        mainMenuUI.SetActive(true);
    }
}
