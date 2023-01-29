using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStatus status = other.GetComponent<PlayerStatus>();
            status.Damage(status.Health * 2);
        }
    }
}
