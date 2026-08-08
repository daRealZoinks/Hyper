using Hyper.ScriptableObjects;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Hyper.UI.Menus
{
    public class CharacterSelectScreen : MonoBehaviour
    {
        public Image characterProfilePicture;
        public TextMeshProUGUI characterName;
        public List<HyperCharacterInfo> hyperCharacterInfo;

        public Button nextCharacterButton;
        public Button previousCharacterButton;

        private bool _isReady;
        private int _currentCharacterIndex;

        public Action OnReadyChanged;

        public HyperCharacterInfo selectedCharacter;

        public bool IsReady
        {
            get
            {
                return _isReady;
            }
            private set
            {
                _isReady = value;
                nextCharacterButton.interactable = !_isReady;
                previousCharacterButton.interactable = !_isReady;
                OnReadyChanged?.Invoke();
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
            selectedCharacter = hyperCharacterInfo[_currentCharacterIndex];

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
    }
}