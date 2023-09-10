using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class BeatIndicator : MonoBehaviour
{
    [Header("Beat Settings")]
    [EventID]
    public string eventID;


    // Koreography Sync with Stance Manager
    private Track track;
    public static Track currentTrack;

    // You can set these materials in the Inspector
    public Material normalMat;
    public Material houseMat;
    public Material technoMat;
    public Material electronicMat;

    private int currentMaterialIndex = 2; // Start with index 2

    private MeshRenderer meshRenderer;

    private void Awake()
    {
        StanceManager.OnStanceChangeStart += StanceManager_OnStanceChange;
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        StanceManager_OnStanceChange(StanceManager.curTrack);
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
        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicEvent);
    }

    private void OnMusicEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        // Reset materials from index 2 to 5 to the normalMat.
        ResetMaterials();

        // Change the material based on the current genre.
        Material material = null;
        switch (currentTrack.genre)
        {
            case Genre.House:
                material = houseMat;
                break;
            case Genre.Techno:
                material = technoMat;
                break;
            case Genre.Electronic:
                material = electronicMat;
                break;
            default:
                material = normalMat;
                break;
        }

        if (material != null)
        {
            // Change the material at the current index.
            if (currentMaterialIndex >= 2 && currentMaterialIndex <= 5)
            {
                Material[] materials = meshRenderer.materials;
                materials[currentMaterialIndex] = material;
                meshRenderer.materials = materials;
            }

            // Increment the index, and loop back to 2 if it exceeds 5.
            currentMaterialIndex++;
            if (currentMaterialIndex > 5)
            {
                currentMaterialIndex = 2;
            }
        }
    }

    private void ResetMaterials()
    {
        Material[] materials = meshRenderer.materials;
        for (int i = 2; i <= 5; i++)
        {
            materials[i] = normalMat;
        }
        meshRenderer.materials = materials;
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
