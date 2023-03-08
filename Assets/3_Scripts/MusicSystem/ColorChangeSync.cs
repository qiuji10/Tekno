using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;

public class ColorChangeSync : MonoBehaviour
{
    [EventID]
    public string eventID;
    public KoreographyTrack Track;

    [SerializeField]
    private Material[] materials;


    [SerializeField]
    private GameObject cube;

    private Renderer cubeRenderer;

    private int randomTextureIndex;
    private int intValueEvt;

    private void Awake()
    {
        cubeRenderer = cube.GetComponent<Renderer>();
        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnBeatChange);
    }

    private void OnBeatChange(KoreographyEvent evt, int sampleTime, int sampleData, DeltaSlice deltaSlice)
    {
        //Getting input color based on Int Range
        intValueEvt = evt.GetIntValue();

        if (intValueEvt == 0)
        {
            cubeRenderer.material = materials[0];
        }
        else if (intValueEvt == 1)
        {
            cubeRenderer.material = materials[1];
        }
        else if (intValueEvt == 2)
        {
            cubeRenderer.material = materials[2];
        }
        else if (intValueEvt == 3)
        {
            cubeRenderer.material = materials[3];
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
