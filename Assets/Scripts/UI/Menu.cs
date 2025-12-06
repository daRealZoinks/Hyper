using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject buttonPrompts;

    public GameObject backPrompt;
    public GameObject selectPrompt;

    public GameObject titleScreen;

    public GameObject mainMenu;
    public GameObject singlePlayerMenu;
    public GameObject multiplayerMenu;
    public GameObject localMenu;
    public GameObject optionsMenu;

    public InputSystemUIInputModule inputSystemUIInputModule;
    public EventSystem eventSystem;

    private readonly Stack<MenuState> menuStack = new();

    private MenuState currentState;


    private readonly Stack<MenuScreen> menuScreenStack = new();
    private MenuScreen currentMenuScreen;

    public enum MenuState
    {
        TitleScreen,
        MainMenu,
        SinglePlayerMenu,
        MultiplayerMenu,
        LocalMenu,
        OptionsMenu
    }

    public MenuState CurrentMenuState
    {
        get
        {
            return currentState;
        }
        set
        {
            currentState = value;

            //buttonPrompts.SetActive(menuStack.Count > 1);

            titleScreen.SetActive(currentState == MenuState.TitleScreen);
            mainMenu.SetActive(currentState == MenuState.MainMenu);
            singlePlayerMenu.SetActive(currentState == MenuState.SinglePlayerMenu);
            multiplayerMenu.SetActive(currentState == MenuState.MultiplayerMenu);
            localMenu.SetActive(currentState == MenuState.LocalMenu);
            optionsMenu.SetActive(currentState == MenuState.OptionsMenu);
        }
    }

    private void OnEnable()
    {
        inputSystemUIInputModule.cancel.action.started += (_) =>
        {
            OnBackButtonPressed();
        };
    }

    private void Update()
    {
        backPrompt.SetActive(menuStack.Count > 1);
        selectPrompt.SetActive(menuStack.Count > 1 && eventSystem.currentSelectedGameObject && eventSystem.currentSelectedGameObject.TryGetComponent<Button>(out var _));
    }

    private void Awake()
    {
        CurrentMenuState = MenuState.TitleScreen;

        menuStack.Push(CurrentMenuState);
    }

    private void SetMenuState(MenuState newState)
    {
        CurrentMenuState = newState;
        menuStack.Push(CurrentMenuState);
    }

    public void OnStartButtonPressed()
    {
        SetMenuState(MenuState.MainMenu);
    }

    public void OnSinglePlayerButtonPressed()
    {
        SetMenuState(MenuState.SinglePlayerMenu);
    }

    public void OnMultiplayerButtonPressed()
    {
        SetMenuState(MenuState.MultiplayerMenu);
    }

    public void OnLocalButtonPressed()
    {
        SetMenuState(MenuState.LocalMenu);
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

            CurrentMenuState = menuStack.Peek();
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
