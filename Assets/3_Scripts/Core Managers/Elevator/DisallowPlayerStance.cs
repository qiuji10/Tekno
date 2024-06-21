using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisallowPlayerStance : MonoBehaviour
{
    [SerializeField] PlayerController controller;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            controller.EnableWithRestrictionNormal();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            controller.EnableActionNormal();
        }
    }
}
