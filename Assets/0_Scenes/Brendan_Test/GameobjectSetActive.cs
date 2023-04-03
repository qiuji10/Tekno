using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameobjectSetActive : MonoBehaviour
{
    public List<GameObject> activeList; // the list of gameobjects to activate when player steps on collider
    public List<GameObject> inactiveList; // the list of gameobjects to deactivate when player steps on collider

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // check if the collider was triggered by the player
        {

            foreach (GameObject obj in inactiveList)
            {
                obj.SetActive(false); // deactivate objects in inactive list
            }

            foreach (GameObject obj in activeList)
            {
                obj.SetActive(true); // activate objects in active list
            }
        }

    }

}
