using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private float distance; // Distance the Spikes need to travel
    private Vector3 startingPosition;
    private float scaleFactor;

    private bool isMoving;
    private int intValueEvt;

    private Track track;
    public static Track currentTrack;

    private void OnEnable()
    {
        StanceManager.OnStanceChangeStart += StanceManager_OnStanceChange;
    }

    private void StanceManager_OnStanceChange(Track obj)
    {
        // Determine which event ID to use based on the track's genre
        if (obj.genre == Genre.House)
        {
            eventID = "120_House_PlatformMove";
            bpm = 120;
            scaleFactor = 0.25f;
        }
        else if (obj.genre == Genre.Techno)
        {
            eventID = "140_Techno_PlatformMove";
            bpm = 140;
            scaleFactor = 1.0f;
        }
        else if (obj.genre == Genre.Electronic)
        {
            eventID = "160_Electro_PlatformMove";
            bpm = 160;
            scaleFactor = 4.0f;
        }

        // Set the current track
        currentTrack = obj;
        Koreographer.Instance.RegisterForEventsWithTime(eventID, RaiseSpikes);
    }

    private void OnDisable()
    {
        StanceManager.OnStanceChangeStart -= StanceManager_OnStanceChange;
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
        distance = Vector3.Distance(startingPosition, targetPosition);
        float duration = ((distance / 60) * bpm) / scaleFactor;

        moveTime += Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveTime / duration);

        if (moveTime >= duration)
        {
            moveTime = 0f;
            isMoving = false;
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
