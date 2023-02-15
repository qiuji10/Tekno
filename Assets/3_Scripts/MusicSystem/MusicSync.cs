using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class MusicSync : MonoBehaviour
{
    [Header("Selected Beat Data")]
    [EventID]
    public string eventID;
    private float scaleFactor; // Scale
    private float speed; // movement speed 
    private float rotation; //rotation
    [Header("How Do you want your Gameobject React To The Music")]
    public bool ScaleSelection;
    public bool RotationSelection;

    private void Awake()
    {
        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicScale);
        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicRotate);
    }

    private void OnMusicScale(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
       if (ScaleSelection)
        {
                scaleFactor = evt.GetValueOfCurveAtTime(sampleTime) * 2.0f;
                transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        }
      
    }
        private void OnMusicRotate(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (RotationSelection)
        {
                rotation = evt.GetValueOfCurveAtTime(sampleTime) * 360.0f;
                transform.localRotation = Quaternion.Euler(0.0f, 0.0f, rotation);
        }
    }
}
