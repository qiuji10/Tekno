using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimEventInvoker : MonoBehaviour
{
    [System.Serializable]
    public class EventsData
    {
        public string eventName;
        public UnityEvent TriggerEvent;
    }

    public List<EventsData> Events;

    public void CallEvent(string eventName)
    {
        foreach (EventsData e in Events)
        {
            if (string.Equals(eventName, e.eventName))
            {
                e.TriggerEvent?.Invoke();
            }
        }
    }
}
