using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class Menu : MonoBehaviour
{
    public GameObject titleScreen;

    public GameObject buttonPrompts;

    public GameObject mainMenu;
    public GameObject singlePlayerMenu;
    public GameObject multiplayerMenu;
    public GameObject optionsMenu;

    private InputSystemUIInputModule _inputModule;

    private readonly Stack<MenuState> menuStack = new();

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

            buttonPrompts.SetActive(currentState != MenuState.TitleScreen);

            titleScreen.SetActive(currentState == MenuState.TitleScreen);
            mainMenu.SetActive(currentState == MenuState.MainMenu);
            singlePlayerMenu.SetActive(currentState == MenuState.SinglePlayerMenu);
            multiplayerMenu.SetActive(currentState == MenuState.MultiplayerMenu);
            optionsMenu.SetActive(currentState == MenuState.OptionsMenu);
        }
    }

    private void OnEnable()
    {
        _inputModule.submit.action.started += (_) =>
        {
            Debug.Log("Submit Detected");
            OnStartButtonPressed();
        };

        _inputModule.leftClick.action.started += (_) =>
        {
            Debug.Log("Left Click Detected");
            OnStartButtonPressed();
        };

        _inputModule.cancel.action.started += (_) =>
        {
            Debug.Log("Cancel Detected");
            OnBackButtonPressed();
        };
    }

    private void Awake()
    {
        _inputModule = GetComponent<InputSystemUIInputModule>();
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

    public void OnBackButtonPressed()
    {
        if (menuStack.Count > 1)
        {
            menuStack.Pop();
            CurrentState = menuStack.Peek();
        }
    }
}
