using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class PlatformFlip : MonoBehaviour
{
    [Header("Payload Settings")]
    [EventID]
    public string payloadEventID;
    public GameObject platform;

    [Header("BPM Settings")]
    public int bpm = 120;
    public float flipSpeed = 1f;

    [Header("Flip Settings")]
    public float flipInterval;
    public float flipDuration;
    private bool flipping = false;


    private GameObject parental;
    public Transform player { get; set; }


    private void Awake()
    {
        parental = transform.GetChild(0).gameObject;
        Koreographer.Instance.RegisterForEventsWithTime(payloadEventID, PayloadTriggered);
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


}

