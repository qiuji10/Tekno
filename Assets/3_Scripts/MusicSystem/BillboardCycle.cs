using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardCycle : MonoBehaviour
{
    [Header("Audio Hijack Settings")]
    [EventID]
    public string eventID;

    public Material[] propagandaMaterials; // Array of materials for normal behavior
    public Material[] hijackedMaterials; // Array of materials for hijacked behavior

    private Track track;
    public static Track currentTrack;

    public bool isHijackedSuccessful = false;

    public int materialIndex = 0; // Index of the material to change (set in the inspector)

    private int currentMaterialIndex = 0; // To track the current material index used

    private void Awake()
    {
        // Set the track field to the current track
        StanceManager_OnStanceChange(StanceManager.curTrack);
        track = currentTrack;
    }

    private void OnEnable()
    {
        StanceManager.OnStanceChangeStart += StanceManager_OnStanceChange;
    }

    private void StanceManager_OnStanceChange(Track obj)
    {
        switch (obj.genre)
        {
            case Genre.House:
                eventID = "120_House_PlatformMove";
                break;
            case Genre.Techno:
                eventID = "140_Techno_Billboards";
                break;
            case Genre.Electronic:
                eventID = "160_Electro_PlatformMove";
                break;
            default:
                eventID = "140_Techno_PlatformMove";
                break;
        }

        // Set the current track
        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicEvent);
    }

    private void OnDisable()
    {
        StanceManager.OnStanceChangeStart -= StanceManager_OnStanceChange;
    }

    private void OnMusicEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (isHijackedSuccessful)
        {
            SetMaterial(currentMaterialIndex, hijackedMaterials);
        }
        else
        {
            SetMaterial(currentMaterialIndex, propagandaMaterials);
        }

        // Increment the currentMaterialIndex and loop back to the start if necessary
        currentMaterialIndex = (currentMaterialIndex + 1) % Mathf.Max(hijackedMaterials.Length, propagandaMaterials.Length);
    }

    private void SetMaterial(int index, Material[] materials)
    {
        if (index >= 0 && index < materials.Length)
        {
            Renderer renderer = GetComponent<Renderer>();
            Material[] rendererMaterials = renderer.materials;
            if (materialIndex < rendererMaterials.Length)
            {
                rendererMaterials[materialIndex] = materials[index];
                renderer.materials = rendererMaterials;
            }
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
