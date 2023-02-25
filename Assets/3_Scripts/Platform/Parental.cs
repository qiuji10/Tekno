using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parental : MonoBehaviour
{
    private PlatformDrop platform;

    private void Awake()
    {
        platform = GetComponentInParent<PlatformDrop>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform.parent); 
            platform.PlayerOnPlatform = true;
            platform.player = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null);
            platform.PlayerOnPlatform = false;
            platform.player = null;
        }
    }
}
