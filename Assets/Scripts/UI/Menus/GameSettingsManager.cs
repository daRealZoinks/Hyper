using Hyper.ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hyper.UI.Menus
{
    public class GameSettingsManager : MonoBehaviour
    {
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

        public List<string> scenes;

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


        public void StartGame()
        {
            // load with some test values, i just wanna make sure this works
            gameMode = GameMode.SingleRace;
            SceneManager.LoadScene(7);

            //playerProfiles[0].characterInfo.characterPrefab;
        }
    }
}