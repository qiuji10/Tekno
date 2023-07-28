using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTVLightRotation : MonoBehaviour
{
    [Header("Rotational Settings")]
    [EventID]
    [SerializeField] private string eventID;
    [SerializeField] private Transform lightTransform;
    [SerializeField] private float rotationSpeed;

    private Track track;
    public static Track currentTrack;

    private void OnEnable()
    {
        StanceManager.OnStanceChangeStart += StanceManager_OnStanceChange;
    }

    private void StanceManager_OnStanceChange(Track obj)
    {
        switch (obj.genre)
        {
            case Genre.House:
                eventID = "120_House_CurvePayload";
                break;
            case Genre.Techno:
                eventID = "140_Techno_CurvePayload";
                break;
            case Genre.Electronic:
                eventID = "160_Electro_CurvePayload";
                break;
            default:
                eventID = "140_Techno_CurvePayload";
                break;
        }

        // Set the current track
        currentTrack = obj;
        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicEvent);
    }

    private void OnDisable()
    {
        StanceManager.OnStanceChangeStart -= StanceManager_OnStanceChange;
    }

    private void Awake()
    {
        StanceManager_OnStanceChange(StanceManager.curTrack);
        track = currentTrack;
    }

    private void OnMusicEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        RotateLight();
    }

    private void RotateLight()
    {
        Quaternion targetRotation = Quaternion.Euler(lightTransform.eulerAngles.x, lightTransform.eulerAngles.y, lightTransform.eulerAngles.z + 90f);

        lightTransform.rotation = Quaternion.RotateTowards(lightTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void OnDestroy()
    {
        if (Koreographer.Instance != null)
        {
            Koreographer.Instance.UnregisterForAllEvents(this);
        }
    }
}
