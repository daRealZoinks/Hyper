using UnityEngine;
using UnityEngine.InputSystem.UI;

public class MultiplayerMenu : MonoBehaviour
{
    public GameObject multiplayerMenu;
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
        multiplayerMenu.SetActive(false);
        mainMenuUI.SetActive(true);
    }
}
