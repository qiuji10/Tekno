using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class PlatformMove : MonoBehaviour
{
    [Header("Movement Settings")]
    [EventID]
    public string eventID;
    public Transform[] points;
    private int currentPoint;
    private bool isMoving;

    [Header("Speed Settings")]
    public int bpm; // Beats per minute
    private float beatDuration; // Duration of one beat in seconds
    private float moveTime;

    private void Awake()
    {
        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicEvent);
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
        float duration = distance /  beatDuration;

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

