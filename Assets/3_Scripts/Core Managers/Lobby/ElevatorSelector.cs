using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorSelector : MonoBehaviour
{
    public void SetDestination(string destination)
    {
        PlayerData.Instance.destination = (ElevatorDestination)Enum.Parse(typeof(ElevatorDestination), destination);
    }
}
