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
    public int carIndex;

    private bool isMoving;
    private float moveTime;
    private float scaleFactor;

    public MeshRenderer carRenderer;

    // Koreography Sync with Stance Manager
    private Track track;
    public static Track currentTrack;

    // IPlatform Interface
    private bool playerOnPlatform;
    private GameObject parental;
    public Transform player { get; set; }
    public bool PlayerOnPlatform { get; set; }

    private void OnEnable()
    {
        StanceManager.OnStanceChangeStart += StanceManager_OnStanceChange;
    }

    private void OnDisable()
    {
        StanceManager.OnStanceChangeStart -= StanceManager_OnStanceChange;
    }

    private void StanceManager_OnStanceChange(Track obj)
    {
        // Determine which event ID to use based on the track's genre
        if (obj.genre == Genre.House)
        {
            eventID = "120_House_MovingCar";
            scaleFactor = 0.075f;
        }
        else if (obj.genre == Genre.Techno)
        {
            eventID = "140_Techno_MovingCar";
            scaleFactor = 0.25f;

        }
        else if (obj.genre == Genre.Electronic)
        {
            eventID = "160_Electro_MovingCar";
            scaleFactor = 0.75f;
        }

        // Set the current track
        currentTrack = obj;
        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicEvent);
    }

    private void Awake()
    {
        StanceManager_OnStanceChange(StanceManager.curTrack);
        track = currentTrack;
    }

    private void Update()
    {
        if (isMoving && !PauseMenu.isPause)
        {
            Vector3 newPosition = waypoints[currentWaypointIndex].position;

            if (!Mathf.Approximately(newPosition.x, float.NaN) && !Mathf.Approximately(newPosition.y, float.NaN) && !Mathf.Approximately(newPosition.z, float.NaN))
                MoveToWaypoint(waypoints[currentWaypointIndex].position);
        }
    }

    private void OnMusicEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        int intValueEvt = evt.GetIntValue();

        if (carIndex == intValueEvt)
        {
            MoveToNextWaypoint();
        }

    }

    private void MoveToNextWaypoint()
    {
        if(!isMoving)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length) { currentWaypointIndex = 0;}

            if (currentWaypointIndex == 0) { carRenderer.enabled = false; }
            else { carRenderer.enabled = true;}

            isMoving = true;
        }
    }

    private void MoveToWaypoint(Vector3 targetPosition)
    {
        moveTime += Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, targetPosition, TempoManager.GetTimeToBeatCount(1) /** scaleFactor*/);

        if (moveTime >= TempoManager.GetTimeToBeatCount(1))
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
