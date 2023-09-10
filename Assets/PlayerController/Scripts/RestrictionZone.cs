using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestrictionZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag(("Player")))
        {
            StanceManager.AllowPlayerSwitchStance = false;
            PlayerController.allowedJump = false;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag(("Player")))
        {
            StanceManager.AllowPlayerSwitchStance = true;
            PlayerController.allowedJump = true;
        }
    }
}
