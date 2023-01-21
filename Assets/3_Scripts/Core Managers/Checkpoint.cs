using UnityEngine;

public class Checkpoint : MonoBehaviour {
    
    public bool pass { get; set; }

    public Transform spawnPoint;

    private void OnTriggerEnter(Collider other) 
    {
        
        if (other.CompareTag("Player"))
        {
            pass = true;
        }

    }

}