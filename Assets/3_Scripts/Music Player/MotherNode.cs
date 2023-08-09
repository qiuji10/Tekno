using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MotherNode : MonoBehaviour
{
    [SerializeField] public List<Transform> teleportPoints = new List<Transform>();
    [SerializeField] private UnityEvent[] events;

    public void InvokeOnSuccess(int numSuccess)
    {
        if(numSuccess < events.Length)
        {
            events[numSuccess].Invoke();
        }
        

    }
}
