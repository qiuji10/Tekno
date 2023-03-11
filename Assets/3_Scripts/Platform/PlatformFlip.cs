using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class PlatformFlip : MonoBehaviour
{
    [Header("Payload Settings")]
    [EventID]
    public string eventID;
    public GameObject platform;

    [Header("Flip Settings")]
    public float flipSpeed = 1f;
    public float flipInterval;
    public float flipDuration;
    private bool flipping = false;

    private int bpm = 140;
    private Track track;
    public static Track currentTrack;

    private GameObject parental;
    public Transform player { get; set; }


    private void Awake()
    {
        // Set the track field to the current track
        track = currentTrack;

        Koreographer.Instance.RegisterForEventsWithTime(eventID, PayloadTriggered);
        flipDuration = 60f / bpm;

        parental = transform.GetChild(0).gameObject;
    }

    private void OnEnable()
    {
        StanceManager.OnStanceChange += StanceManager_OnStanceChange;
    }

    private void StanceManager_OnStanceChange(Track obj)
    {
        // Determine which event ID to use based on the track's genre
        if (obj.genre.ToString() == "House")
        {
            eventID = "120_House_IntPayload";
            bpm = 120;
        }
        else if (obj.genre.ToString() == "Techno")
        {
            eventID = "140_Techno_IntPayload";
            bpm = 140;
        }
        else if (obj.genre.ToString() == "Electronic")
        {
            eventID = "160_Electro_IntPayload";
            bpm = 160;
        }

        // Set the current track
        currentTrack = obj;

        Koreographer.Instance.RegisterForEventsWithTime(eventID, PayloadTriggered);
    }

    private void OnDisable()
    {
        StanceManager.OnStanceChange -= StanceManager_OnStanceChange;
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

    private void PayloadTriggered(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaslice)
    {
        if(!flipping)
        {
            StartCoroutine(Flip());
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

