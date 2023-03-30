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

    private int bpm = 140;
    private int intValueEvt;

    private GameObject parental;
    public Transform player { get; set; }

    // Add OnEnable implementation
    private void OnEnable()
    {
        StanceManager.OnStanceChange += StanceManager_OnStanceChange;
    }

    // Add OnDisable implementation
    private void OnDisable()
    {
        StanceManager.OnStanceChange -= StanceManager_OnStanceChange;
    }

    // Add OnStanceChange implementation
    private void StanceManager_OnStanceChange(Track obj)
    {
        if (obj.genre == Genre.House)
        {
            EventID = "120_House_IntPayload";
            bpm = 120;
        }
        else if (obj.genre == Genre.Techno)
        {
            EventID = "140_Techno_IntPayload";
            bpm = 140;
        }
        else if (obj.genre == Genre.Electronic)
        {
            EventID = "160_Electro_IntPayload";
            bpm = 160;
        }

        Koreographer.Instance.RegisterForEventsWithTime(EventID, SwitchPlatform);
    }

    private void Awake()
    {
        parental = transform.GetChild(0).gameObject;
        Koreographer.Instance.RegisterForEventsWithTime(EventID, SwitchPlatform);
    }

    private void SwitchPlatform(KoreographyEvent evt, int sampleTime, int sampleData, DeltaSlice deltaSlice)
    {
        //Getting Evt int from Koreography
        intValueEvt = evt.GetIntValue();

        if (intValueEvt == 0)
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
