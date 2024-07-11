using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool pass;
    public static Checkpoint latest; 

    public Transform spawnPoint;

    private void OnTriggerEnter(Collider other) 
    {
        if (!pass && other.CompareTag("Player"))
        {
            latest = this;
            pass = true;
        }
    }

}