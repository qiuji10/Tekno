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

    private bool isMoving;

    [Header("Speed Settings")]
    public int bpm; // Beats per minute
    private float beatDuration; // Duration of one beat in seconds
    private float moveTime;

    private int intValueEvt;

    private void Awake()
    {
        Koreographer.Instance.RegisterForEventsWithTime(eventID, RaiseSpikes);
        beatDuration = 60f / bpm;
    }

    private void Update()
    {
        
    }

    private void RaiseSpikes(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        intValueEvt = evt.GetIntValue();

        //Spikes Moving to Position Based on Koreography Evt Int
        if(intValueEvt == 0)
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
        float distance = Vector3.Distance(transform.position, targetPosition);
        float duration = distance / beatDuration;

        moveTime += Time.deltaTime;
        Spikes.transform.position = Vector3.Lerp(Spikes.transform.position, targetPosition, moveTime / duration);

        if (moveTime >= duration)
        {
            moveTime = 0f;
        }
    }
}
