using UnityEngine;

public class PlayerReturn: MonoBehaviour
{
    public GameObject playerObject;
    private CheckpointManager checkpointManager;
    private Transform playerTransform;

    private void Start()
    {
        // Get references to the CheckpointManager and player Transform
        checkpointManager = FindObjectOfType<CheckpointManager>();
        playerTransform = playerObject.transform;
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
