using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameobjectSetActive : MonoBehaviour
{
    public List<GameObject> activeList; // the list of gameobjects to activate when player steps on collider
    public List<GameObject> inactiveList; // the list of gameobjects to deactivate when player steps on collider
    public float dissolveDuration = 1f;// the duration of the dissolve animation in seconds

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // check if the collider was triggered by the player
        {
            StartCoroutine(DissolveInactiveList()); // dissolve the inactive game objects
        }
    }

    IEnumerator DissolveInactiveList()
    {
        // Dissolve inactiveList game objects
        foreach (GameObject obj in inactiveList)
        {
            DissolveController dissolveController = obj.GetComponent<DissolveController>();
            if (dissolveController != null)
            {
                dissolveController.SetDissolveAmount(1f, dissolveDuration); // dissolve from 0 to 1
            }
        }

        yield return new WaitForSeconds(dissolveDuration); // wait for the dissolve animation to finish

        foreach (GameObject obj in inactiveList)
        {
            obj.SetActive(false); // deactivate the object
        }

        // Dissolve activeList game objects
        foreach (GameObject obj in activeList)
        {
            obj.SetActive(true); // activate objects in active list
            DissolveController dissolveController = obj.GetComponent<DissolveController>();
            if (dissolveController != null)
            {
                dissolveController.SetDissolveAmount(0f, dissolveDuration); // dissolve from 1 to 0
            }
        }
    }
}



