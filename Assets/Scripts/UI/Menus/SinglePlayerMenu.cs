using UnityEngine;
using UnityEngine.InputSystem.UI;

public class SinglePlayerMenu : MonoBehaviour
{
    public GameObject singlePlayerMenu;
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
        singlePlayerMenu.SetActive(false);
        mainMenuUI.SetActive(true);
    }
}
