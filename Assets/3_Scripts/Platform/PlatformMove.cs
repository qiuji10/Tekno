using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using UnityEditor.Experimental.GraphView;

public class PlatformMove : MonoBehaviour
{
    [EventID]
    public string eventID;
    public KoreographyTrack Track;
    public Transform[] points;
    private Transform objTransform;
    private float movementSpeed;
    private float bpm;
    private int currentPoint;

    private void Awake()
    {
        objTransform = GetComponent<Transform>();
        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicSpeed);
    }

    private void Start()
    {
        //bpm = Track.BPM;
    }

    private void OnMusicSpeed(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        //updating moving speed based on BPM
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

//[Header("Moving Platform settings")]
//public Transform[] points;
//private int currentPoint = 0;
//public float timeToPoint = 7f;

//private void FixedUpdate()
//{
//    MovePlatform();
//}

//public void MovePlatform()
//{
//    if (isMoveable || isMoveable && isDropable)
//    {
//        Vector3 dir = (points[currentPoint].position - transform.position).normalized;
//        transform.position = transform.position + dir * dropDistance * Time.deltaTime;

//if (Vector3.Distance(transform.position, points[currentPoint].position) < 0.1f)
//{
//    currentPoint++;

//    if (currentPoint >= points.Length)
//    {
//        currentPoint = 0;
//    }
//}
//    }

//}
