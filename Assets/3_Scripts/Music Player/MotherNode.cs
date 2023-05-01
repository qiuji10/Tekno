using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotherNode : MonoBehaviour
{
    [SerializeField] public List<Transform> teleportPoints = new List<Transform>();
    [SerializeField] private TeleportAbility teleportAbility;
    //private int currentNodeIndex = 0;
    private bool playerInRange = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            teleportAbility.motherNode = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void canTeleport()
    {
        if(playerInRange &&  teleportAbility != null)
        {
            StartCoroutine(TeleportToNodes());
        }
    }
    public IEnumerator TeleportToNodes()
    {
        foreach (Transform teleportPoint in teleportPoints)
        {
            if (teleportPoint != null && teleportPoint.gameObject.activeInHierarchy)
            {
                teleportAbility.TeleportToNextNode(teleportPoint);
                yield return new WaitForSeconds(0.15f); // wait for the player to finish teleporting before teleporting again
            }
        }
    }
}
