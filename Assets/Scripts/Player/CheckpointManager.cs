using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    public List<Checkpoint> checkpointList = new();

    public LinkedList<Checkpoint> checkpointLinkedList = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        checkpointLinkedList = new(checkpointList);
    }
}
