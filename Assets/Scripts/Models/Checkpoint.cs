using Assets.Scripts.Constants;
using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private UnityEvent OnCheckpointSet;
    [SerializeField] private bool isDefaultCheckpoint = false;
    [SerializeField] private bool setViaCollision = true;

    private void Awake()
    {
        if (this.isDefaultCheckpoint)
            CheckpointManager.SetCheckpoint(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!this.setViaCollision)
            return;

        if (collision.CompareTag(Tags.PLAYER) && CheckpointManager.Instance.CurrentCheckpoint != this)
        {
            CheckpointManager.SetCheckpoint(this);
            this.OnCheckpointSet?.Invoke();
        }
    }
}
