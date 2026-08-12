using Hyper.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace Hyper.UI.Menus
{
    public static class PlayerSettingsManager
    {
        public static Dictionary<PlayerInput, HyperCharacterInfo> players = new();

        public static void SpawnPlayers()
        {
            foreach (var player in players)
            {
                if (player.Key.devices.Count == 1)
                {
                    PlayerInput.Instantiate(player.Value.characterPrefab, player.Key.playerIndex, player.Key.currentActionMap.controlSchemes[0].name, player.Key.splitScreenIndex, player.Key.devices[0]);
                }
                else
                {
                    if (player.Key.devices.Count == 2)
                    {
                        {
                            PlayerInput.Instantiate(player.Value.characterPrefab, player.Key.playerIndex, player.Key.currentActionMap.controlSchemes[0].name, player.Key.splitScreenIndex, player.Key.devices[0], player.Key.devices[1]);
                        }
                    }
                }
            }
        }
    }
}