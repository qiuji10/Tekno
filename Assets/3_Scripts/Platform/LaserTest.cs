using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTest : MonoBehaviour
{
    [SerializeField] private LineRenderer laser;
    [SerializeField] private Transform startPoint1;
    [SerializeField] private Transform startPoint2;
    
    [SerializeField] private Transform endPoint1;
    [SerializeField] private Transform endPoint2;
    [SerializeField] public float laserWidth = 0.1f;

    [EventID]
    public string eventID;

    private Vector3 currentStartPoint;
    private Vector3 currentEndPoint;

    private float lerpTime = 1f;
    private float currentLerpTime;

    private void Start()
    {
        currentStartPoint = startPoint1.position;
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

            Vector3 newStartPoint = Vector3.zero;
            Vector3 newEndPoint = Vector3.zero;

            if (payload == 0)
            {
                newStartPoint = startPoint1.position;
                newEndPoint = endPoint1.position;
            }
            else if (payload == 1)
            {
                newStartPoint = startPoint2.position;
                newEndPoint = endPoint2.position;
            }
            
            Vector3 startPosBottom = laser.GetPosition(0);
            Vector3 startPosTop = laser.GetPosition(1);
            currentLerpTime = 0f;
            StartCoroutine(LerpLaser(startPosBottom, newStartPoint, lerpTime, 0));
            StartCoroutine(LerpLaser(startPosTop, newEndPoint, lerpTime, 1));
        }
    }

    private IEnumerator LerpLaser(Vector3 startPos, Vector3 endPos, float duration, int laserPoint)
    {
        while (currentLerpTime < duration)
        {
            currentLerpTime += Time.deltaTime;
            float t = currentLerpTime / duration;

            //Lerp the position of the Line Renderer
            Vector3 lerpedPos = Vector3.Lerp(startPos, endPos, t);
            laser.SetPosition(laserPoint, lerpedPos);

            yield return null;
        }

        laser.SetPosition(laserPoint, endPos);
    }

    private void Update()
    {
        // Set the laser start point and width
        //laser.SetPosition(0, currentStartPoint);
        laser.widthMultiplier = laserWidth;
    }
}


