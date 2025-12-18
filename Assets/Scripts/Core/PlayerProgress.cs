using System.Collections.Generic;
using UnityEngine;

namespace Hyper.Core
{
    public class PlayerProgress : MonoBehaviour
    {
        public LinkedListNode<Checkpoint> lastReachedCheckpoint;

        public int lapsCompleted;
        public int place;

        private void Awake()
        {
            RaceManager.Instance.RegisterPlayer(this);
        }

        public void SetCheckpoint(LinkedListNode<Checkpoint> checkpointNode)
        {
            if (checkpointNode.Value == RaceManager.Instance.startingCheckpoint)
            {
                if (lastReachedCheckpoint == null)
                {
                    lapsCompleted = 0;
                }
                else
                {
                    if (checkpointNode == lastReachedCheckpoint.NextOrFirst())
                    {
                        LapCompleted();
                    }
                }
            }

            lastReachedCheckpoint = checkpointNode;
        }

        private void LapCompleted()
        {
            lapsCompleted++;
        }
    }

    public static class LinkedListNodeExtentions
    {
        public static LinkedListNode<T> NextOrFirst<T>(this LinkedListNode<T> current)
        {
            return current.Next ?? current.List.First;
        }

        public static LinkedListNode<T> PreviousOrLast<T>(this LinkedListNode<T> current)
        {
            return current.Previous ?? current.List.Last;
        }
    }
}