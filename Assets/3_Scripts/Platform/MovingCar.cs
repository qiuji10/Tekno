using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class MovingCar : MonoBehaviour
{
    [Header("Moving Platform Settings")]
    [EventID]
    public string eventID;
    public Transform[] waypoints;
    private int currentWaypointIndex;
    private bool isMoving;

    // Koreography Sync with Stance Manager
    private Track track;
    public static Track currentTrack;

    // IPlatform Interface
    private bool playerOnPlatform;
    private GameObject parental;
    public Transform player { get; set; }
    public bool PlayerOnPlatform { get; set; }

    private int carIndex;

    private void OnEnable()
    {
        StanceManager.OnStanceChangeStart += StanceManager_OnStanceChange;
    }

    private void StanceManager_OnStanceChange(Track obj)
    {
        // Determine which event ID to use based on the track's genre
        if (obj.genre == Genre.House)
        {
            eventID = "120_House_MovingCar";
        }
        else if (obj.genre == Genre.Techno)
        {
            eventID = "140_Techno_MovingCar";
        }
        else if (obj.genre == Genre.Electronic)
        {
            eventID = "160_Electro_MovingCar";
        }

        // Set the current track
        currentTrack = obj;
        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicEvent);
    }

    private void OnDisable()
    {
        StanceManager.OnStanceChangeStart -= StanceManager_OnStanceChange;
    }

    private void Awake()
    {
        StanceManager_OnStanceChange(StanceManager.curTrack);
        track = currentTrack;
    }

    private void Update()
    {
        if (isMoving)
        {
            MoveToWaypoint(waypoints[currentWaypointIndex].position);
        }
    }

    private void OnMusicEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        int targetCarIndex = evt.GetIntValue();

        if (targetCarIndex == carIndex)
        {
            MoveToNextWaypoint();
        }
    }

    private void MoveToNextWaypoint()
    {
        currentWaypointIndex++;
        if (currentWaypointIndex >= waypoints.Length)
        {
            currentWaypointIndex = 0;
        }

        isMoving = true;
    }

    private void MoveToWaypoint(Vector3 targetPosition)
    {
        float moveSpeed = 5f;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (transform.position == targetPosition)
        {
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
