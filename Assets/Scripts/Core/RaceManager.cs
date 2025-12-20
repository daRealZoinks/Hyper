using System.Collections.Generic;
using UnityEngine;

namespace Hyper.Core
{
    public class RaceManager : MonoBehaviour
    {
        public static RaceManager Instance { get; private set; }

        public Checkpoint startingCheckpoint;

        private readonly List<PlayerProgress> playerProgressList = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            SortPlayerPositions();

            for (int i = 0; i < playerProgressList.Count; i++)
            {
                playerProgressList[i].place = i + 1;
            }
        }

        private void SortPlayerPositions()
        {
            for (int i = 0; i < playerProgressList.Count - 1; i++)
            {
                bool swapped = false;

                for (int j = 0; j < playerProgressList.Count - i - 1; j++)
                {
                    var player1Progress = playerProgressList[j];
                    var player2Progress = playerProgressList[j + 1];

                    var player1LastCheckpointIndex = 0;
                    var player2LastCheckpointIndex = 0;

                    if (player1Progress.lastReachedCheckpoint != null)
                    {
                        player1LastCheckpointIndex = CheckpointManager.Instance.checkpointList.FindIndex((checkpoint) => checkpoint == player1Progress.lastReachedCheckpoint.Value);
                    }

                    if (player2Progress.lastReachedCheckpoint != null)
                    {
                        player2LastCheckpointIndex = CheckpointManager.Instance.checkpointList.FindIndex((checkpoint) => checkpoint == player2Progress.lastReachedCheckpoint.Value);
                    }

                    if (player1Progress.lapsCompleted == player2Progress.lapsCompleted)
                    {
                        if (player1LastCheckpointIndex == player2LastCheckpointIndex)
                        {
                            var player1position = player1Progress.transform.position;
                            var player2position = player2Progress.transform.position;

                            var player1nextCheckpointPosition = startingCheckpoint.transform.position;

                            if (player1Progress.lastReachedCheckpoint != null)
                            {
                                player1nextCheckpointPosition = player1Progress.lastReachedCheckpoint.NextOrFirst().Value.transform.position;
                            }

                            var player2nextCheckpointPosition = startingCheckpoint.transform.position;

                            if (player2Progress.lastReachedCheckpoint != null)
                            {
                                player2nextCheckpointPosition = player2Progress.lastReachedCheckpoint.NextOrFirst().Value.transform.position;
                            }

                            var player1DistanceTowardsNextCheckpoint = Vector3.Distance(player1position, player1nextCheckpointPosition);
                            var player2DistanceTowardsNextCheckpoint = Vector3.Distance(player2position, player2nextCheckpointPosition);

                            if (player1DistanceTowardsNextCheckpoint > player2DistanceTowardsNextCheckpoint)
                            {
                                playerProgressList[j] = player2Progress;
                                playerProgressList[j + 1] = player1Progress;
                                swapped = true;
                            }
                        }
                        else
                        {
                            if (player1LastCheckpointIndex < player2LastCheckpointIndex)
                            {
                                playerProgressList[j] = player2Progress;
                                playerProgressList[j + 1] = player1Progress;
                                swapped = true;
                            }
                        }
                    }
                    else
                    {
                        if (player1Progress.lapsCompleted < player2Progress.lapsCompleted)
                        {
                            playerProgressList[j] = player2Progress;
                            playerProgressList[j + 1] = player1Progress;
                            swapped = true;
                        }
                    }
                }

                if (!swapped)
                {
                    break;
                }
            }
        }

        public void RegisterPlayer(PlayerProgress playerProgress)
        {
            playerProgressList.Add(playerProgress);
        }
    }
}