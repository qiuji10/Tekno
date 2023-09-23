using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class BeatIndTest : MonoBehaviour
{
    [Header("Beat Settings")]
    [EventID]
    public string eventID;
    public MeshRenderer indicatorMesh; // Reference to the Mesh Renderer.

    private Track track;
    public static Track currentTrack;

    [Header("Stance Materials")]
    [SerializeField] private Material baseMat;
    [SerializeField] private Material houseMat;
    [SerializeField] private Material technoMat;
    [SerializeField] private Material electroMat;

    private int currentMaterialIndex = 0;

    private void Awake()
    {
        StanceManager.OnStanceChangeStart += StanceManager_OnStanceChange;
        track = currentTrack;
    }

    private void Start()
    {
        StanceManager_OnStanceChange(StanceManager.curTrack);
    }

    private void StanceManager_OnStanceChange(Track obj)
    {
        // Set the current track
        currentTrack = obj;

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

        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicEvent);
    }

    private void OnMusicEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (evt.HasIntPayload())
        {
            int intValue = evt.GetIntValue();

            // Check if the intValue is within a valid range
            if (intValue >= 0 && intValue <= 3)
            {
                // Determine the new material index based on the integer value and player's stance.
                int newMaterialIndex = intValue;

                // Change the material.
                ChangeMaterial(newMaterialIndex);

                // Reset the previous material back to the base material.
                ResetMaterial(currentMaterialIndex);

                // Update the current material index.
                currentMaterialIndex = newMaterialIndex - 1;
                if (currentMaterialIndex == -1) { currentMaterialIndex = 3; }
            }
        }
    }

    private void ChangeMaterial(int index)
    {
        // Determine the material based on the current genre
        Material material;
        switch (currentTrack.genre)
        {
            case Genre.House:
                material = houseMat;
                break;
            case Genre.Techno:
                material = technoMat;
                break;
            case Genre.Electronic:
                material = electroMat;
                break;
            default:
                material = baseMat;
                break;
        }

        // Access the MeshRenderer's materials array and change the material at the specified index.
        if (indicatorMesh != null)
        {
            Material[] materials = indicatorMesh.materials;
            materials[index] = material;
            indicatorMesh.materials = materials;
        }
    }

    private void ResetMaterial(int index)
    {
        // Reset the material back to the base material.
        if (indicatorMesh != null)
        {
            Material[] materials = indicatorMesh.materials;
            materials[index] = baseMat;
            indicatorMesh.materials = materials;
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
