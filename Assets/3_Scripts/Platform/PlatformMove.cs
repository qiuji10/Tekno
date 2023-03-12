using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class PlatformMove : MonoBehaviour, IPlatform
{ 
    [Header("Moving Platform Settings")]
    [EventID]
    public string eventID;
    public Transform[] points;
    private int currentPoint;

    private bool isMoving;
    private int bpm = 140; // Beats per minute
    private float beatDuration; // Duration of one beat in seconds
    public float moveTime;

    private Track track;
    public static Track currentTrack;

    private GameObject parental;
    public Transform player { get; set; }
    public bool PlayerOnPlatform { get; set; }

    private void OnEnable()
    {
        StanceManager.OnStanceChange += StanceManager_OnStanceChange;
    }

    private void StanceManager_OnStanceChange(Track obj)
    {
        // Determine which event ID to use based on the track's genre
        if (obj.genre == Genre.House)
        {
            eventID = "120_House_PlatformMove";
            bpm = 120;
            beatDuration = 60f / bpm;
        }
        else if (obj.genre == Genre.Techno)
        {
            eventID = "140_Techno_PlatformMove";
            bpm = 140;
            beatDuration = 60f / bpm;
        }
        else if (obj.genre == Genre.Electronic)
        {
            eventID = "160_Electro_PlatformMove";
            bpm = 160;
            beatDuration = 60f / bpm;
        }

        // Set the current track
        currentTrack = obj;
        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicEvent);
    }

    private void OnDisable()
    {
        StanceManager.OnStanceChange -= StanceManager_OnStanceChange;
    }

    private void Awake()
    {
        // Set the track field to the current track
        track = currentTrack;
        beatDuration = 60f / bpm;
    }

    private void Update()
    {
        if (isMoving)
        {
            MoveToPoint(points[currentPoint].position);
        }
    }

    private void OnMusicEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        // Move object to next point on beat
        if (!isMoving)
        {
            currentPoint++;
            if (currentPoint >= points.Length)
            {
                currentPoint = 0;
            }

            isMoving = true;
        }
    }

    private void MoveToPoint(Vector3 targetPosition)
    {
        float distance = Vector3.Distance(transform.position, targetPosition);
        float duration = distance / beatDuration;

        moveTime += Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveTime / duration);

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
