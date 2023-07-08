using UnityEngine;

public class PlayerReturn : MonoBehaviour
{
    [SerializeField]
    private Transform teleportDestination;
    private Transform playerTransform;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TeleportPlayer();
        }
    }

    public void TeleportPlayer()
    {
        playerTransform.position = teleportDestination.position;
    }
}
