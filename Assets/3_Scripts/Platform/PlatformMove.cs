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
    private float moveTime;
    private float scaleFactor;

    //Koreography Sync with Stance Manager
    private Track track;
    public static Track currentTrack;

    //IPlatform Interface
    private bool playerOnPlatform;
    private GameObject parental;
    public Transform player { get; set; }
    public bool PlayerOnPlatform {get; set;}

    private void OnEnable()
    {
        StanceManager.OnStanceChangeStart += StanceManager_OnStanceChange;
    }

    private void StanceManager_OnStanceChange(Track obj)
    {
        switch (obj.genre)
        {
            case Genre.House:
                eventID = "120_House_PlatformMove";
                scaleFactor = 0.15f;
                break;
            case Genre.Techno:
                eventID = "140_Techno_PlatformMove";
                scaleFactor = 0.1f;
                break;
            case Genre.Electronic:
                eventID = "160_Electro_PlatformMove";
                break;
            default:
                eventID = "140_Techno_PlatformMove";
                scaleFactor = 0.05f;
                break;
        }

        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicEvent);
    }

    private void OnDisable()
    {
        StanceManager.OnStanceChangeStart -= StanceManager_OnStanceChange;
    }

    private void Start()
    {
        StanceManager_OnStanceChange(StanceManager.curTrack);
    }

    private void Update()
    {
        if (isMoving && !PauseMenu.isPause)
        {
            Vector3 newPosition = points[currentPoint].position;

            if (!Mathf.Approximately(newPosition.x, float.NaN) && !Mathf.Approximately(newPosition.y, float.NaN) && !Mathf.Approximately(newPosition.z, float.NaN))
                MoveToPoint(points[currentPoint].position);
        }
    }

    private void OnMusicEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        // Move object to next point on beat
        if (!isMoving)
        {
            currentPoint++;
            //Reset to 0 if exceed Index Array
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
        float duration = (distance / TempoManager.GetTimeToBeatCount(1)) * scaleFactor;

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