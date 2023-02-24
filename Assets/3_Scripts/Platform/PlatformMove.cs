using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using UnityEngine.UI;

public class PlatformMove : MonoBehaviour
{
    [Header("Select platform Type")]
    public bool movingPlatform = false;
    public bool flippingPlatform = false;

    [Header("Platform Settings")]
    [EventID]
    public string eventID;
    public Transform[] points;
    private Transform objTransform;
    private float movementSpeed;
    private float flippingSpeed;
    private int currentPoint;

    [Header("Flip Settings")]
    public float flipInterval = 5f; // The time between flips
    public float flipDuration = 1f;
    public float timeSinceLastFlip;
    private bool flipping = false;

    public Transform player { get; set; }
    private GameObject parental;


    private void Awake()
    {
        objTransform = GetComponent<Transform>();
        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicMove);
    }

    private void Start()
    {
        //bpm = Track.BPM;
    }

    private void OnMusicMove(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if(movingPlatform)
        {
            //updating moving speed based on event curvedata
            movementSpeed = evt.GetValueOfCurveAtTime(sampleTime);

            //Move platform according to beat 
            Vector3 dir = (points[currentPoint].position - transform.position).normalized;
            transform.position = transform.position + dir * movementSpeed;

            if (Vector3.Distance(transform.position, points[currentPoint].position) < 0.1f)
            {
                currentPoint++;

                if (currentPoint >= points.Length)
                {
                    currentPoint = 0;
                }
            }
        }
    }

    public void OnMusicFlip(KoreographyEvent evt, int sampleTime, int sampleData, DeltaSlice deltaSlice)
    {
        if (flippingPlatform)
        {
            //Flipping speed based on event curvedata
            flippingSpeed = evt.GetValueOfCurveAtTime(sampleTime);

            //Flip platform 
            if (!flipping)
            {
                timeSinceLastFlip += Time.deltaTime;
                if (timeSinceLastFlip >= flipInterval)
                {
                    StartCoroutine(Flip());

                    timeSinceLastFlip = 0;
                }
            }
        }

        IEnumerator Flip()
        {
            parental.SetActive(false);
            if (player != null && player.parent != null) player.SetParent(null);
            flipping = true;
            Quaternion startRotation = transform.rotation;
            Quaternion endRotation = startRotation * Quaternion.Euler(180f, 0f, 0f);
            float time = 0f;

            while (time <= flipDuration)
            {
                time += Time.deltaTime;
                transform.rotation = Quaternion.Lerp(startRotation, endRotation, time / flipDuration);

                yield return null;
            }
            flipping = false;
            parental.SetActive(true);
        }

    }
}
