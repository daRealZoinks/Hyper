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
                // You can add additional logic here to transition to the next scene or start the game.
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