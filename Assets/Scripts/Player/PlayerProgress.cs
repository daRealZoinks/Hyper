using System.Collections.Generic;
using UnityEngine;

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
                if (checkpointNode == lastReachedCheckpoint.Next)
                {
                    LapCompleted();
                }
            }
        }
        else
        {
            if (checkpointNode == checkpointNode.List.Last)
            {
                // need to loop back to start
            }
        }

        lastReachedCheckpoint = checkpointNode;
    }

    private void LapCompleted()
    {
        lapsCompleted++;
    }
}