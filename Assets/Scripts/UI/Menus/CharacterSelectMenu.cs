using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Hyper.UI.Menus
{
    public class CharacterSelectMenu : MonoBehaviour
    {
        public Action OnAllPlayersReady;

        public List<PlayerInput> players;

        private GridLayoutGroup _gridLayoutGroup;

        private void Awake()
        {
            _gridLayoutGroup = GetComponentInChildren<GridLayoutGroup>();

            OnAllPlayersReady += () =>
            {
                Debug.Log("All players are ready!");

                foreach (var player in players)
                {
                    var characterSelectScreen = player.GetComponent<CharacterSelectScreen>();

                    if (player.devices.Count == 1)
                    {
                        PlayerInput.Instantiate(characterSelectScreen.selectedCharacter.characterPrefab, player.playerIndex, player.currentControlScheme, player.splitScreenIndex, player.devices[0]);
                    }
                    else
                    {
                        if (player.devices.Count == 2)
                        {
                            {
                                PlayerInput.Instantiate(characterSelectScreen.selectedCharacter.characterPrefab, player.playerIndex, player.currentControlScheme, player.splitScreenIndex, player.devices[0], player.devices[1]);
                            }
                        }
                    }
                }
            };
        }

        public void OnPlayerJoined(PlayerInput playerInput)
        {
            players.Add(playerInput);

            playerInput.transform.SetParent(_gridLayoutGroup.transform);

            playerInput.GetComponent<CharacterSelectScreen>().OnReadyChanged += () =>
            {
                foreach (var player in players)
                {
                    if (!player.GetComponent<CharacterSelectScreen>().IsReady)
                    {
                        return;
                    }
                }

                OnAllPlayersReady?.Invoke();
            };
        }

        public void OnPlayerLeft(PlayerInput playerInput)
        {
            players.Remove(playerInput);
            Destroy(playerInput.gameObject);
        }
    }
}