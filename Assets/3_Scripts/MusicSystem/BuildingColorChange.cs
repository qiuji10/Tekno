using NaughtyAttributes;
using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingColorChange : MonoBehaviour
{
    [Header("Audio Hijack Settings")]
    [EventID]
    public string eventID;

    public Material[] ElectroMat;
    public Material[] HouseMat;
    public Material[] TechnoMat;

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
                StartCoroutine(ColorChangeDelayHouse());
                SetRandomMaterial(HouseMat);
                break;
            case Genre.Techno:
                eventID = "140_Techno_Billboards";
                StartCoroutine(ColorChangeDelayTechno());
                SetRandomMaterial(TechnoMat);
                break;
            case Genre.Electronic:
                eventID = "160_Electro_Billboards";
                StartCoroutine(ColorChangeDelayElectro());
                SetRandomMaterial(ElectroMat);
                break;
            default:
                eventID = "140_Techno_Billboards";
                StartCoroutine(ColorChangeDelayTechno());
                SetRandomMaterial(HouseMat);
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
        SetRandomMaterial(TechnoMat);
    }

    private void OnMusicEvent(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {


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

    private IEnumerator ColorChangeDelayHouse()
    {
        yield return new WaitForSeconds(2.85f);

        SetRandomMaterial(HouseMat);

    }

    private IEnumerator ColorChangeDelayTechno()
    {
        yield return new WaitForSeconds(2.85f);

        SetRandomMaterial(TechnoMat);

    }

    private IEnumerator ColorChangeDelayElectro()
    {
        yield return new WaitForSeconds(2.85f);

        SetRandomMaterial(ElectroMat);

    }
}
