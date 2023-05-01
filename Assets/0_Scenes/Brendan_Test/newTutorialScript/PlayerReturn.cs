using UnityEngine;

public class PlayerReturn: MonoBehaviour
{
    private CheckpointManager checkpointManager;
    private Transform playerTransform;

    private void Start()
    {
        // Get references to the CheckpointManager and player Transform
        checkpointManager = FindObjectOfType<CheckpointManager>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Get the latest passed checkpoint from the CheckpointManager
            Checkpoint latestCheckpoint = checkpointManager.GetLatestCheckpoint();

            // Set the player's position to the latest passed checkpoint's spawn point
            playerTransform.position = latestCheckpoint.spawnPoint.position;
        }
    }

}
