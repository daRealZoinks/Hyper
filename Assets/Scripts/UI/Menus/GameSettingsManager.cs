using System;
using System.Collections.Generic;
using Hyper.ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hyper.UI.Menus
{
    public class GameSettingsManager : MonoBehaviour
    {
        [Serializable]
        public enum NetworkMode
        {
            SinglePlayer,
            LocalMultiplayer,
            Online,
        }

        public NetworkMode networkMode;

        [Serializable]
        public class PlayerProfile
        {
            public string name;
            public HyperCharacterInfo characterInfo;
        }

        public List<PlayerProfile> playerProfiles;

        [Serializable]
        public enum GameMode
        {
            GrandPrix,
            SingleRace,
            Knockout,
        }

        public GameMode gameMode;

        public List<Scene> scenes;

        private void SetNetworkMode(NetworkMode networkMode)
        {
            this.networkMode = networkMode;
        }

        public void SetNetworkModeSinglePlayer()
        {
            SetNetworkMode(NetworkMode.SinglePlayer);
        }

        public void SetNetworkModeLocalMultiplayer()
        {
            SetNetworkMode(NetworkMode.LocalMultiplayer);
        }

        public void SetNetworkModeOnline()
        {
            SetNetworkMode(NetworkMode.Online);
        }

        private void SetGameMode(GameMode gameMode)
        {
            this.gameMode = gameMode;
        }

        public void SetGameModeGrandPrix()
        {
            SetGameMode(GameMode.GrandPrix);
        }

        public void SetGameModeSingleRace()
        {
            SetGameMode(GameMode.SingleRace);
        }

        public void SetGameModeKnockout()
        {
            SetGameMode(GameMode.Knockout);
        }
    }
}