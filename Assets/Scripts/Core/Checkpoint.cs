using UnityEngine;

namespace Hyper.Core
{
    public class Checkpoint : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (other.TryGetComponent<PlayerProgress>(out var playerInterface))
                {
                    playerInterface.SetCheckpoint(CheckpointManager.Instance.checkpointLinkedList.Find(this));
                }
            }
        }
    }
}