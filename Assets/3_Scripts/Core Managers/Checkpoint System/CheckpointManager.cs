using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;

public class CheckpointManager : MonoBehaviour {

    [SerializeField] private List<Checkpoint> checkpoints;

    public Checkpoint GetLatestCheckpoint()
    {
        return Checkpoint.latest;
    }

    /// This may be no needed based on certain situation
    public void SetPassedCheckpoints(int checkpointsPassed)
    {
        for (int i = 0; i < checkpointsPassed; i++)
        {
            checkpoints[i].pass = true;
        }
    }

    public void SetPlayerToSpawnPoint(Transform player)
    {
        player.position = GetLatestCheckpoint().spawnPoint.position;
    }

#if UNITY_EDITOR
    [Button]
    public void AssignCheckpointsReference()
    {
        checkpoints.Clear();
        checkpoints = FindObjectsOfType<Checkpoint>().ToList();
    }
#endif

}