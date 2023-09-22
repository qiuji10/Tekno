using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class BeatIndTest : MonoBehaviour
{
    [Header("Beat Settings")]
    [EventID]
    public string eventID;

    [SerializeField] private GameObject movingPlatform;
    [SerializeField] private MeshRenderer platformRenderer;

    private Track track;
    public static Track currentTrack;

    [Header("Stance Materials")]
    [SerializeField] private Material baseMat;
    [SerializeField] private Material houseMat;
    [SerializeField] private Material technoMat;
    [SerializeField] private Material electroMat;

    private int startIndex = 2;
    private int endIndex = 5;
    private int currentIndex;

    private void Awake()
    {
        StanceManager.OnStanceChangeStart += StanceManager_OnStanceChange;
        track = currentTrack;
        platformRenderer = movingPlatform.GetComponent<MeshRenderer>();
    }

    private void StanceManager_OnStanceChange(Track obj)
    {
        switch (obj.genre)
        {
            case Genre.House:
                eventID = "120_House_MovingCar";
                break;
            case Genre.Techno:
                eventID = "140_Techno_MovingCar";
                break;
            case Genre.Electronic:
                eventID = "160_Electro_MovingCar";
                break;
            default:
                eventID = "140_Tekno_MovingCar";
                break;
        }

        // Set the current track
        currentTrack = obj;
        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicEvent);
    }

    private void OnMusicEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (evt.HasIntPayload())
        {
            int intValue = evt.GetIntValue();

            if (intValue >= 0 && intValue <= 3)
            {

            }
        }
    }

    private void OnDestroy()
    {
        if (Koreographer.Instance != null)
        {
            Koreographer.Instance.UnregisterForAllEvents(this);
        }

        StanceManager.OnStanceChangeStart -= StanceManager_OnStanceChange;
    }
}
