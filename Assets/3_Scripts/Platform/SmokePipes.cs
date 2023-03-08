using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokePipes : MonoBehaviour
{
    [Header("Door Settings")]
    public GameObject leftDoor;

    [Header("Event Settings")]
    [EventID]
    public string eventID;
    private bool isOpen = false;
    private Quaternion leftDoorRotationOpen;
    private Quaternion leftDoorRotationClose;

    [Header("Speed Settings")]
    public float rotateDuration = 0.5f;
    private float rotateTime;

    private Coroutine rotationCoroutine = null;
    public ParticleSystem smokeParticles;

    private void Awake()
    {
        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicEvent);
        leftDoorRotationOpen = leftDoor.transform.rotation;
        leftDoorRotationClose = Quaternion.Euler(0, -90f, 0);

    }

    private void OnMusicEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (evt.GetIntValue() == 1)
        {
            if (!isOpen)
            {
                rotationCoroutine = StartCoroutine(RotateDoorsOpen(leftDoor, -90f));
                smokeParticles.Play();
                isOpen = true;
            }
        }
        else if (evt.GetIntValue() == 0)
        {
            if (isOpen)
            {
                rotationCoroutine = StartCoroutine(RotateDoorsClose(leftDoor, 0f));
                isOpen = false;
                smokeParticles.Stop();
            }
        }
    }

    private IEnumerator RotateDoorsOpen(GameObject leftDoorObj, float leftAngle)
    {
        Quaternion leftDoorEndRotation = Quaternion.Euler(leftAngle, 0f, 0f);

        float startTime = Time.time;
        while (Time.time < startTime + rotateDuration)
        {
            float t = (Time.time - startTime) / rotateDuration;
            leftDoorObj.transform.localRotation = Quaternion.Slerp(leftDoorRotationOpen, leftDoorEndRotation, t);
            yield return null;
        }

        leftDoorObj.transform.localRotation = leftDoorEndRotation;

        rotationCoroutine = null;
    }

    private IEnumerator RotateDoorsClose(GameObject leftDoorObj, float leftAngle)
    {
        Quaternion leftDoorEndRotation = Quaternion.Euler(leftAngle, 0f, 0f);

        float startTime = Time.time;
        while (Time.time < startTime + rotateDuration)
        {
            float t = (Time.time - startTime) / rotateDuration;
            leftDoorObj.transform.localRotation = Quaternion.Slerp(leftDoorRotationClose, leftDoorEndRotation, t);
            yield return null;
        }

        leftDoorObj.transform.localRotation = leftDoorEndRotation;

        rotationCoroutine = null;
    }

    private void OnDestroy()
    {
        if (Koreographer.Instance != null)
        {
            Koreographer.Instance.UnregisterForAllEvents(this);
        }

        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
        }
    }
}
