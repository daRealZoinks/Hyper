using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class Menu : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject mainMenu;
    public GameObject singlePlayerMenu;
    public GameObject multiplayerMenu;
    public GameObject optionsMenu;

    public InputSystemUIInputModule inputModule;

    private readonly Stack<MenuState> menuStack = new();

    [SerializeField]
    private MenuState currentState;

    public enum MenuState
    {
        TitleScreen,
        MainMenu,
        SinglePlayerMenu,
        MultiplayerMenu,
        OptionsMenu
    }

    public MenuState CurrentState
    {
        get
        {
            return currentState;
        }
        set
        {
            currentState = value;

            titleScreen.SetActive(currentState == MenuState.TitleScreen);
            mainMenu.SetActive(currentState == MenuState.MainMenu);
            singlePlayerMenu.SetActive(currentState == MenuState.SinglePlayerMenu);
            multiplayerMenu.SetActive(currentState == MenuState.MultiplayerMenu);
            optionsMenu.SetActive(currentState == MenuState.OptionsMenu);
        }
    }

    private void OnEnable()
    {
        inputModule.submit.action.started += (_) =>
        {
            OnStartButtonPressed();
        };

        inputModule.cancel.action.started += (_) =>
        {
            OnBackButtonPressed();
        };
    }

    private void Start()
    {
        CurrentState = MenuState.TitleScreen;
        menuStack.Push(CurrentState);
    }

    private void SetMenuState(MenuState newState)
    {
        CurrentState = newState;
        menuStack.Push(CurrentState);
    }

    public void OnStartButtonPressed()
    {
        if (CurrentState == MenuState.TitleScreen)
        {
            SetMenuState(MenuState.MainMenu);
        }
    }

    public void OnSinglePlayerButtonPressed()
    {
        SetMenuState(MenuState.SinglePlayerMenu);
    }

    public void OnMultiplayerButtonPressed()
    {
        SetMenuState(MenuState.MultiplayerMenu);
    }

    public void OnOptionsButtonPressed()
    {
        SetMenuState(MenuState.OptionsMenu);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OnBackButtonPressed()
    {
        if (menuStack.Count > 1)
        {
            menuStack.Pop();
            CurrentState = menuStack.Peek();
        }
    }
}
