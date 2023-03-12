using System.Collections;
using UnityEngine;
using SonicBloom.Koreo;

public class TrapDoors : MonoBehaviour
{
    [Header("Door Settings")]
    public GameObject leftDoor;
    public GameObject rightDoor;

    [Header("Event Settings")]
    [EventID]
    public string eventID;

    private bool isOpen = false;
    private Quaternion leftDoorRotationOpen;
    private Quaternion rightDoorRotationOpen;
    private Quaternion leftDoorRotationClose;
    private Quaternion rightDoorRotationClose;

    [Header("Speed Settings")]
    public float rotateDuration = 0.5f;
    private float rotateTime;

    private Coroutine rotationCoroutine = null;
    public ParticleSystem smokeParticles;

    private int bpm = 140; // Beats per minute
    private Track track;
    public static Track currentTrack;


    private void Awake()
    {
        // Set the track field to the current track
        track = currentTrack;

        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicEvent);
        rotateDuration = 60f / bpm;

        leftDoorRotationOpen = leftDoor.transform.rotation;
        rightDoorRotationOpen = rightDoor.transform.rotation;
        leftDoorRotationClose = Quaternion.Euler(-90f,0,0);
        rightDoorRotationClose = Quaternion.Euler(90f, 0, 0);
    }

    private void OnEnable()
    {
        StanceManager.OnStanceChange += StanceManager_OnStanceChange;
    }

    private void StanceManager_OnStanceChange(Track obj)
    {
        // Determine which event ID to use based on the track's genre
        if (obj.genre == Genre.House)
        {
            eventID = "120_House_IntPayload";
            bpm = 120;
            rotateDuration = 60f / bpm;
        }
        else if (obj.genre == Genre.Techno)
        {
            eventID = "140_Techno_IntPayload";
            bpm = 140;
            rotateDuration = 60f / bpm;
        }
        else if (obj.genre == Genre.Electronic)
        {
            eventID = "160_Electro_IntPayload";
            bpm = 160;
            rotateDuration = 60f / bpm;
        }

        // Set the current track
        currentTrack = obj;
        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicEvent);
    }

    private void OnDisable()
    {
        StanceManager.OnStanceChange -= StanceManager_OnStanceChange;
    }

    private void OnMusicEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (evt.GetIntValue() == 1)
        {
            if (!isOpen)
            {
                rotationCoroutine = StartCoroutine(RotateDoorsOpen(leftDoor, -90f, rightDoor, 90f));
                smokeParticles.Play();
                isOpen = true;
            }
        }
        else if (evt.GetIntValue() == 0)
        {
            if (isOpen)
            {
                rotationCoroutine = StartCoroutine(RotateDoorsClose(leftDoor, 0f, rightDoor, 0f));
                isOpen = false;
                smokeParticles.Stop();
            }
        }
    }

    private IEnumerator RotateDoorsOpen(GameObject leftDoorObj, float leftAngle, GameObject rightDoorObj, float rightAngle)
    {
        Quaternion leftDoorEndRotation = Quaternion.Euler(leftAngle, 0f, 0f);
        Quaternion rightDoorEndRotation = Quaternion.Euler(rightAngle, 0f, 0f);

        float startTime = Time.time;
        while (Time.time < startTime + rotateDuration)
        {
            float t = (Time.time - startTime) / rotateDuration;
            leftDoorObj.transform.localRotation = Quaternion.Slerp(leftDoorRotationOpen, leftDoorEndRotation, t);
            rightDoorObj.transform.localRotation = Quaternion.Slerp(rightDoorRotationOpen, rightDoorEndRotation, t);
            yield return null;
        }

        leftDoorObj.transform.localRotation = leftDoorEndRotation;
        rightDoorObj.transform.localRotation = rightDoorEndRotation;

        rotationCoroutine = null;
    }

    private IEnumerator RotateDoorsClose(GameObject leftDoorObj, float leftAngle, GameObject rightDoorObj, float rightAngle)
    {
        Quaternion leftDoorEndRotation = Quaternion.Euler(leftAngle, 0f, 0f);
        Quaternion rightDoorEndRotation = Quaternion.Euler(rightAngle, 0f, 0f);

        float startTime = Time.time;
        while (Time.time < startTime + rotateDuration)
        {
            float t = (Time.time - startTime) / rotateDuration;
            leftDoorObj.transform.localRotation = Quaternion.Slerp(leftDoorRotationClose, leftDoorEndRotation, t);
            rightDoorObj.transform.localRotation = Quaternion.Slerp(rightDoorRotationClose, rightDoorEndRotation, t);
            yield return null;
        }

        leftDoorObj.transform.localRotation = leftDoorEndRotation;
        rightDoorObj.transform.localRotation = rightDoorEndRotation;

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

