using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorSelector : MonoBehaviour
{
    private PlayerData data;

    private void Start()
    {
        data = FindObjectOfType<PlayerData>();
    }

    public void SetDestination(string destination)
    {
        data.destination = (ElevatorDestination)Enum.Parse(typeof(ElevatorDestination), destination);
    }
}
