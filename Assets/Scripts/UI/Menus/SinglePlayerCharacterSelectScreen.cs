using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Hyper.UI.Menus
{
    public class SinglePlayerCharacterSelectScreen : MonoBehaviour
    {
        public Image characterProfilePicture;
        public TextMeshProUGUI characterName;
        public List<HyperCharacterInfo> hyperCharacterInfo;

        public Button nextCharacterButton;
        public Button previousCharacterButton;

        private bool _isReady;
        private int _currentCharacterIndex;

        public bool IsReady
        {
            get
            {
                return _isReady;
            }
            set
            {
                _isReady = value;
                nextCharacterButton.interactable = !_isReady;
                previousCharacterButton.interactable = !_isReady;
            }
        }

        private void Start()
        {
            UpdateCharacterDisplay();
        }

        public void NextCharacter()
        {
            if (!IsReady)
            {
                _currentCharacterIndex = (_currentCharacterIndex + 1) % hyperCharacterInfo.Count;
                UpdateCharacterDisplay();
            }
        }

        public void PreviousCharacter()
        {
            if (!IsReady)
            {
                _currentCharacterIndex = (_currentCharacterIndex - 1 + hyperCharacterInfo.Count) % hyperCharacterInfo.Count;
                UpdateCharacterDisplay();
            }
        }

        private void UpdateCharacterDisplay()
        {
            var characterInfo = hyperCharacterInfo[_currentCharacterIndex];
            characterProfilePicture.sprite = characterInfo.characterProfilePicture;
            characterName.text = characterInfo.characterName;
        }

        public void ToggleReady()
        {
            IsReady = !IsReady;
        }

        public void ChooseCharacter(InputAction.CallbackContext context)
        {
            if (context.performed && !IsReady)
            {
                var value = context.ReadValue<Vector2>();
                _currentCharacterIndex = (_currentCharacterIndex + (int)value.normalized.x + hyperCharacterInfo.Count) % hyperCharacterInfo.Count;
                UpdateCharacterDisplay();
            }
        }

        public void ReadyUp(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                IsReady = true;
            }
        }

        public void Unready(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                IsReady = false;
            }
        }
    }
}