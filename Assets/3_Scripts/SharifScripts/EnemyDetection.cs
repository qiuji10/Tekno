using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public GameObject[] enemiesInArea;
    public GameObject closeEnemy;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider that entered the trigger is an enemy
        if (other.CompareTag("Enemy"))
        {
            float minDistance = float.MaxValue;

            // Find the closest enemy to the trigger
            foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                float distance = Vector3.Distance(enemy.transform.position, transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closeEnemy = enemy;
                }
            }

            // Do something with the closest enemy
            Debug.Log("Closest enemy is: " + closeEnemy.name);
        }

        Debug.Log(other.name);
    }

    
}
