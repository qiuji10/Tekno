using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class BeatIndicator : MonoBehaviour
{
    [Header("Beat Settings")]
    [EventID]
    public string eventID;
    public GameObject[] sequence;
    private int currentBeat;

    // Koreography Sync with Stance Manager
    private Track track;
    public static Track currentTrack;

    public Material normalMat;
    public Material houseMaterial;
    public Material technoMaterial;
    public Material electronicMaterial;

    private void OnEnable()
    {
        StanceManager.OnStanceChangeStart += StanceManager_OnStanceChange;
    }

    private void OnDisable()
    {
        StanceManager.OnStanceChangeStart -= StanceManager_OnStanceChange;
    }

    private void Awake()
    {
        // Set the track field to the current track
        StanceManager_OnStanceChange(StanceManager.curTrack);
        track = currentTrack;
    }

    private void StanceManager_OnStanceChange(Track obj)
    {
        // Determine which event ID to use based on the track's genre
        if (obj.genre == Genre.House)
        {
            eventID = "120_House_MovingCar";
        }
        else if (obj.genre == Genre.Techno)
        {
            eventID = "140_Techno_MovingCar";
        }
        else if (obj.genre == Genre.Electronic)
        {
            eventID = "160_Electro_MovingCar";
        }

        // Set the current track
        currentTrack = obj;
        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicEvent);
    }

    private void OnMusicEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        int intValueEvt = evt.GetIntValue();

        // Check if the intValueEvt is within the valid range of the sequence array
        if (intValueEvt >= 0 && intValueEvt < sequence.Length)
        {
            // Reset the previous beat's material
            if (currentBeat >= 0 && currentBeat < sequence.Length)
            {
                ResetMaterial(currentBeat);
            }

            // Change the material of the current beat
            ChangeMaterial(intValueEvt);
            currentBeat = intValueEvt;
        }
    }

    private void ChangeMaterial(int index)
    {
        // Determine the material based on the current genre
        Material material;
        switch (currentTrack.genre)
        {
            case Genre.House:
                material = houseMaterial;
                break;
            case Genre.Techno:
                material = technoMaterial;
                break;
            case Genre.Electronic:
                material = electronicMaterial;
                break;
            default:
                material = null;
                break;
        }

        // Change the material of the object at the specified index in the sequence array
        if (material != null)
        {
            sequence[index].GetComponent<Renderer>().material = material;
        }
    }

    private void ResetMaterial(int index)
    {
        // Reset the material of the object at the specified index in the sequence array
        sequence[index].GetComponent<Renderer>().material = normalMat;
    }

    private void OnDestroy()
    {
        if (Koreographer.Instance != null)
        {
            Koreographer.Instance.UnregisterForAllEvents(this);
        }
    }
}
