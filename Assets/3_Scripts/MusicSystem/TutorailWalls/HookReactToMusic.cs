using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;


public class HookReactToMusic : MonoBehaviour
{
    [Header("Selected Beat Data")]
    [EventID]
    public string eventID;
    [SerializeField] private Material material;
    private Color originalColor;
    Color gridColor = new Color(1.72079539f, 1.57664502f, 0, 0);
    private void OnEnable()
    {
        StanceManager.OnStanceChange += StanceManager_OnStanceChange;
    }

    private void OnDisable()
    {
        StanceManager.OnStanceChange -= StanceManager_OnStanceChange;

    }

    private void Awake()
    {

        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicReact);
    }

    private void StanceManager_OnStanceChange(Track obj)
    {

        Koreographer.Instance.UnregisterForEvents(eventID, OnMusicReact);
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
        }

        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicReact);
    }

    private void OnMusicReact(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        float intensity = evt.GetValueOfCurveAtTime(sampleTime);
        int multiplier = 2; 
        
        material.SetColor("_EmissionColor", gridColor * intensity * multiplier);


    }

}
