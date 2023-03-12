using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialWallsOnBeat : MonoBehaviour
{
    [Header("Selected Beat Data")]
    [EventID]
    public string eventID;
    [SerializeField] private Material material;
    [SerializeField] private Material materialStatic;
    [SerializeField] private Material materialStatic1;
    [SerializeField] private Material materialStatic2;
    private Color originalColor;
    Color gridColor = Color.clear;
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
                gridColor = new Color(1.72079539f, 1.57664502f, 0, 0);
                break;

            case Genre.Techno:
                eventID = "140_Techno_CurvePayload";
                gridColor = new Color(0, 0.205526888f, 3.92452836f, 0);
                break;

            case Genre.Electronic:
                eventID = "160_Electro_CurvePayload";
                 gridColor = new Color(0.0313725509f, 1.74117649f, 0, 0);
                break;

        }

        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicReact);
    }

    private void OnMusicReact(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        float intensity = evt.GetValueOfCurveAtTime(sampleTime);
        material.SetColor("_GridColor", gridColor * intensity);
        materialStatic.SetColor("_GridColor", gridColor * intensity);

    }


}

