using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTest : MonoBehaviour
{
    [SerializeField] private LineRenderer laser;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint1;
    [SerializeField] private Transform endPoint2;
    [SerializeField] private float laserWidth = 0.1f;

    [EventID]
    public string eventID;

    private Vector3 currentStartPoint;
    private Vector3 currentEndPoint;

    private float lerpTime = 0.5f;
    private float currentLerpTime;

    private void Start()
    {
        currentStartPoint = startPoint.position;
        currentEndPoint = endPoint1.position;

        laser.SetPosition(0, currentStartPoint);
        laser.SetPosition(1, currentEndPoint);
    }

    private void Awake()
    {
        Koreographer.Instance.RegisterForEventsWithTime(eventID, SwitchLaserEndpoint);
    }


    private void SwitchLaserEndpoint(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (evt.HasIntPayload())
        {
            int payload = evt.GetIntValue();

            Vector3 newEndPoint = Vector3.zero;

            if (payload == 0)
            {
                newEndPoint = endPoint1.position;
            }
            else if (payload == 1)
            {
                newEndPoint = endPoint2.position;
            }
            
            Vector3 startPos = laser.GetPosition(1);
            currentLerpTime = 0f;
            StartCoroutine(LerpLaser(startPos, newEndPoint, lerpTime));
        }
    }

    private IEnumerator LerpLaser(Vector3 startPos, Vector3 endPos, float duration)
    {
        while (currentLerpTime < duration)
        {
            currentLerpTime += Time.deltaTime;
            float t = currentLerpTime / duration;

            //Lerp the position of the Line Renderer
            Vector3 lerpedPos = Vector3.Lerp(startPos, endPos, t);
            laser.SetPosition(1, lerpedPos);

            yield return null;
        }

        laser.SetPosition(1, endPos);
    }

    private void Update()
    {
        // Set the laser start point and width
        laser.SetPosition(0, currentStartPoint);
        laser.widthMultiplier = laserWidth;
    }
}


