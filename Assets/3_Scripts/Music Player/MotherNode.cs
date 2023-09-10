using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MotherNode : MonoBehaviour
{
    [SerializeField] public List<Transform> teleportPoints = new List<Transform>();
    [SerializeField] private UnityEvent[] events;

    private void Awake()
    {
        if (teleportPoints.Count <= 0)
        {
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void InvokeOnSuccess(int numSuccess)
    {
        if(numSuccess < events.Length)
        {
            events[numSuccess].Invoke();
        }
        

    }
}
