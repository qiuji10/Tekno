using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikedPlatform : MonoBehaviour
{
    [Header("Payload Settings")]
    [EventID]
    public string eventID;
    public GameObject Spikes;
    public Transform initialPoint;
    public Transform finalPoint;

    private int bpm = 140; // Beats per minute
    private float beatDuration; // Duration of one beat in seconds
    private float moveTime;
    private float distanceToTravel; // Distance the Spikes need to travel
    private Vector3 startingPosition;

    private bool isMoving;
    private int intValueEvt;

    private Track track;
    public static Track currentTrack;

    private void OnEnable()
    {
        StanceManager.OnStanceChange += StanceManager_OnStanceChange;
    }

    private void StanceManager_OnStanceChange(Track obj)
    {
        // Determine which event ID to use based on the track's genre
        if (obj.genre.ToString() == "House")
        {
            eventID = "120_House_IntPayload";
            bpm = 120;
        }
        else if (obj.genre.ToString() == "Techno")
        {
            eventID = "140_Techno_IntPayload";
            bpm = 140;
        }
        else if (obj.genre.ToString() == "Electronic")
        {
            eventID = "160_Electro_IntPayload";
            bpm = 160;
        }

        // Set the current track
        currentTrack = obj;

        Koreographer.Instance.RegisterForEventsWithTime(eventID, RaiseSpikes);
    }

    private void OnDisable()
    {
        StanceManager.OnStanceChange -= StanceManager_OnStanceChange;
    }

    private void Awake()
    {
        // Set the track field to the current track
        track = currentTrack;

        Koreographer.Instance.RegisterForEventsWithTime(eventID, RaiseSpikes);
        beatDuration = 60f / bpm;
    }

    private void RaiseSpikes(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        intValueEvt = evt.GetIntValue();

        //Spikes Moving to Position Based on Koreography Evt Int
        if (intValueEvt == 0)
        {
            MoveToPoint(initialPoint.position);
        }
        else if (intValueEvt == 1)
        {
            MoveToPoint(finalPoint.position);
        }
    }

    private void MoveToPoint(Vector3 targetPosition)
    {
        startingPosition = Spikes.transform.position;
        distanceToTravel = Vector3.Distance(startingPosition, targetPosition);
        float duration = distanceToTravel / beatDuration;

        moveTime += Time.deltaTime;
        float t = moveTime / duration;
        Spikes.transform.position = Vector3.Lerp(startingPosition, targetPosition, t);

        if (moveTime >= duration)
        {
            moveTime = 0f;
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
