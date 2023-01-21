using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parental : MonoBehaviour
{

    private Platform platform;

    private void Awake()
    {
        platform = GetComponentInParent<Platform>();

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
