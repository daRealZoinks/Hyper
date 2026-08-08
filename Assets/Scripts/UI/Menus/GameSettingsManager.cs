using Hyper.ScriptableObjects;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hyper.UI.Menus
{
    public class GameSettingsManager : MonoBehaviour
    {
        [Serializable]
        public enum GameMode
        {
            GrandPrix,
            SingleRace,
            Knockout,
        }

        public GameMode gameMode;

        public string characterSelectMenuSceneName = "Character Select Menu Test";

        [HideInInspector]
        public MapInfo mapInfo;

        // TODO: grand prix

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public void SetGameMode(GameMode gameMode)
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

        public void SetMap(MapInfo map)
        {
            mapInfo = map;
        }

        public void StartGame()
        {
            switch (gameMode)
            {
                case GameMode.GrandPrix:
                    // TODO: Implement Grand Prix mode logic
                    break;
                case GameMode.SingleRace or GameMode.Knockout:
                    SceneManager.LoadScene(characterSelectMenuSceneName);
                    SceneManager.LoadScene(mapInfo.mapName, LoadSceneMode.Additive);
                    break;
                default:
                    Debug.LogError("Invalid game mode selected.");
                    break;
            }
        }
    }
}