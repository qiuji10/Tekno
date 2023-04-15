using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotherNode : MonoBehaviour
{
    [SerializeField] public List<TeleportNode> teleportNodes = new List<TeleportNode>();
    [SerializeField] private TeleportAbility teleportAbility;
    private int currentNodeIndex = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (teleportNodes.Count > 0)
            {
                StartCoroutine(TeleportToNodes());
            }
        }
    }

    public IEnumerator TeleportToNodes()
    {
        foreach (TeleportNode node in teleportNodes)
        {
            if (node != null && node.gameObject.activeInHierarchy)
            {
                Transform targetTeleportPoint = (node.nextTeleportPoint != null) ? node.nextTeleportPoint : node.prevTeleportPoint;
                teleportAbility.TeleportToNextNode(targetTeleportPoint);
                yield return new WaitForSeconds(0.15f); // wait for the player to finish teleporting before teleporting again
            }
        }
    }
}
