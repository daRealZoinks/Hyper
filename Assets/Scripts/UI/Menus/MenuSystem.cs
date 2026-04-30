using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Hyper.UI.Menus
{
    public class MenuSystem : MonoBehaviour
    {
        private readonly Stack<MenuScreen> menuScreenStack = new();

        public MenuScreen titleScreen;

        public Button backButton;

        private void Awake()
        {
            PushMenuScreen(titleScreen);

            EventSystem.current.GetComponent<InputSystemUIInputModule>().cancel.action.started += (_) =>
            {
                PopMenuScreen();
            };
        }

        public void PushMenuScreen(MenuScreen menuScreen)
        {
            if (menuScreenStack.Count > 0)
            {
                menuScreenStack.Peek().gameObject.SetActive(false);
            }
            menuScreenStack.Push(menuScreen);
            menuScreen.gameObject.SetActive(true);

            UpdateBackButton();
        }

        public void PopMenuScreen()
        {
            if (menuScreenStack.Count > 1)
            {
                menuScreenStack.Pop().gameObject.SetActive(false);
                menuScreenStack.Peek().gameObject.SetActive(true);
            }

            UpdateBackButton();
        }

        private void UpdateBackButton()
        {
            if (backButton != null)
            {
                backButton.gameObject.SetActive(menuScreenStack.Count > 1);
            }
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}