using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingPlatform : MonoBehaviour
{
    [Header("Payload Settings")]
    [EventID]
    public string EventID;
    public GameObject Platform1;
    public GameObject Platform2;

    [Header("BPM Settings")]
    public int bpm = 0;

    private int intValueEvt;

    private GameObject parental;
    public Transform player { get; set; }

    private void Awake()
    {
        parental = transform.GetChild(0).gameObject;
        Koreographer.Instance.RegisterForEventsWithTime(EventID, SwitchPlatform);
    }

    private void SwitchPlatform(KoreographyEvent evt, int sampleTime, int sampleData, DeltaSlice deltaSlice)
    {
        //Getting Evt int from Koreography
        intValueEvt = evt.GetIntValue();

        if(intValueEvt == 0)
        {
            Platform1.gameObject.SetActive(true);
            Platform2.gameObject.SetActive(false);
        }
        else if (intValueEvt == 1)
        {
            Platform1.gameObject.SetActive(false);
            Platform2.gameObject.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        if (Koreographer.Instance != null)
        {
            Koreographer.Instance.UnregisterForAllEvents(this);
        }
    }


}
