using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DelayEvent : MonoBehaviour
{
    public UnityEvent Event;

    public void InvokeEvent()
    {
        Invoke("EventInvoke", 1f);
    }

    private void EventInvoke()
    {
        Event?.Invoke();
    }
}
