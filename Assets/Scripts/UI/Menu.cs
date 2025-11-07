using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject titleScreen;

    public GameObject buttonPrompts;

    public GameObject mainMenu;
    public GameObject singlePlayerMenu;
    public GameObject multiplayerMenu;
    public GameObject optionsMenu;

    private InputSystemUIInputModule _inputModule;
    private EventSystem _eventSystem;

    private readonly Stack<KeyValuePair<MenuState, Button>> menuButtonStack = new();

    private MenuState currentState;

    public enum MenuState
    {
        TitleScreen,
        MainMenu,
        SinglePlayerMenu,
        MultiplayerMenu,
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
        _inputModule.cancel.action.started += (_) =>
        {
            OnBackButtonPressed();
        };
    }

    private void Awake()
    {
        _inputModule = GetComponent<InputSystemUIInputModule>();
        _eventSystem = GetComponent<EventSystem>();
        CurrentMenuState = MenuState.TitleScreen;
        menuButtonStack.Push(new KeyValuePair<MenuState, Button>(CurrentMenuState, _eventSystem.currentSelectedGameObject.GetComponent<Button>()));
    }

    private void SetMenuState(MenuState newState, Button button)
    {
        CurrentMenuState = newState;
        menuButtonStack.Push(new KeyValuePair<MenuState, Button>(newState, button));
    }

    public void OnStartButtonPressed()
    {
        SetMenuState(MenuState.MainMenu, _eventSystem.currentSelectedGameObject.GetComponent<Button>());
    }

    public void OnSinglePlayerButtonPressed()
    {
        SetMenuState(MenuState.SinglePlayerMenu, _eventSystem.currentSelectedGameObject.GetComponent<Button>());
    }

    public void OnMultiplayerButtonPressed()
    {
        SetMenuState(MenuState.MultiplayerMenu, _eventSystem.currentSelectedGameObject.GetComponent<Button>());
    }

    public void OnOptionsButtonPressed()
    {
        SetMenuState(MenuState.OptionsMenu, _eventSystem.currentSelectedGameObject.GetComponent<Button>());
    }

    public void OnBackButtonPressed()
    {
        if (menuButtonStack.Count > 1)
        {
            menuButtonStack.Pop();

            var lastMenuState = menuButtonStack.Peek();

            CurrentMenuState = lastMenuState.Key;
            lastMenuState.Value.Select();
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
