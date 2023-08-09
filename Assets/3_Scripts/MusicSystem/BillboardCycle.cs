using NaughtyAttributes;
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

    public bool isHijackedSuccessful = false;

    public int materialIndex = 0; // Index of the material to change (set in the inspector)

    private Track track;
    public static Track currentTrack;

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
                eventID = "120_House_Billboards";
                break;
            case Genre.Techno:
                eventID = "140_Techno_Billboards";
                break;
            case Genre.Electronic:
                eventID = "160_Electro_Billboards";
                break;
            default:
                eventID = "140_Techno_Billboards";
                break;
        }

        // Set the current track
        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicEvent);
    }

    private void OnDisable()
    {
        StanceManager.OnStanceChangeStart -= StanceManager_OnStanceChange;
    }

    [Button]
    private void SetHijackMaterial()
    {
        SetRandomMaterial(hijackedMaterials);
    }

    private void OnMusicEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (isHijackedSuccessful)
        {
            SetRandomMaterial(hijackedMaterials);
        }
        else
        {
            SetRandomMaterial(propagandaMaterials);
        }
    }

    private void SetRandomMaterial(Material[] materials)
    {
        if (materials != null && materials.Length > 0)
        {
            List<Renderer> childRenderers = new List<Renderer>();
            GetChildRenderers(transform, childRenderers);

            foreach (Renderer renderer in childRenderers)
            {
                if (materialIndex < renderer.sharedMaterials.Length)
                {
                    int randomIndex = Random.Range(0, materials.Length);
                    Material[] newMaterials = renderer.sharedMaterials;
                    newMaterials[materialIndex] = materials[randomIndex];
                    renderer.sharedMaterials = newMaterials;
                }
            }
        }
    }

    private void GetChildRenderers(Transform parent, List<Renderer> renderers)
    {
        renderers.AddRange(parent.GetComponents<Renderer>());

        for (int i = 0; i < parent.childCount; i++)
        {
            GetChildRenderers(parent.GetChild(i), renderers);
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
