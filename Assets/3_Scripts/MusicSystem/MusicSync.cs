using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class MusicSync : MonoBehaviour
{
    [Header("Selected Beat Data")]
    [EventID]
    public string eventID;
    public GameObject[] sequence;
    // Koreography Sync with Stance Manager
    private Track track;
    public static Track currentTrack;
    public Material normalMat;
    public Material houseMaterial;
    public Material technoMaterial;
    public Material electronicMaterial;

    private int currentBeat;

    private Vector3 scale = Vector3.one;
    private float speed; // movement speed 
    [Header("How Do you want your Gameobject React To The Music")]
    public bool ScaleSelection;
    public bool RotationSelection;
    [Header("Scale Settings")]
    public bool ScaleX;
    public bool ScaleY;
    public bool ScaleZ;
    [Header("Rotation Options")]
    public bool RotateX;
    public bool RotateY;
    public bool RotateZ;
    public Vector3 scaleMod = Vector3.one;
    private Vector3 originalScale;
    private Quaternion originalRotation;

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
        // Save the original scale and rotation for resetting later
        originalScale = transform.localScale;
        originalRotation = transform.localRotation;
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


            if (ScaleSelection)
            {
                Vector3 scale = originalScale;
                scale.x = ScaleX ? evt.GetValueOfCurveAtTime(sampleTime) * 2.0f * scaleMod.x : scale.x;
                scale.y = ScaleY ? evt.GetValueOfCurveAtTime(sampleTime) * 2.0f * scaleMod.y : scale.y;
                scale.z = ScaleZ ? evt.GetValueOfCurveAtTime(sampleTime) * 2.0f * scaleMod.z : scale.z;
                transform.localScale = scale;
            }

            if (RotationSelection)
            {
                Quaternion rotation = originalRotation;
                float x = RotateX ? evt.GetValueOfCurveAtTime(sampleTime) * 360.0f : 0.0f;
                float y = RotateY ? evt.GetValueOfCurveAtTime(sampleTime) * 360.0f : 0.0f;
                float z = RotateZ ? evt.GetValueOfCurveAtTime(sampleTime) * 360.0f : 0.0f;
                rotation = Quaternion.Euler(x, y, z);
                transform.localRotation = rotation;
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
