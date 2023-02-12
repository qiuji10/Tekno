using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class MusicSync : MonoBehaviour
{
    [Header("Selected Beat Data")]
    [EventID]
    public string eventID;
    private Transform transform;
    private float scaleFactor; // Scale
    private float speed; // movement speed 
    private float rotation; //rotation
    [Header("How Do you want your Gameobject React To The Music")]
    public bool ScaleSelection;
    public bool RotationSelection;

    private void Awake()
    {
        transform = GetComponent<Transform>(); //get transform from game object
        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicScale);
        //Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicSpeed);
        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicRotate);
    }

    private void OnMusicScale(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
       if (ScaleSelection)
        {
                // Update the scale factor based on the beat amplitude.
                scaleFactor = evt.GetValueOfCurveAtTime(sampleTime) * 2.0f;
                Debug.Log("MusicSync: Scale factor: " + scaleFactor);

                // Update the transform scale.
                transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        }
      
    }
        private void OnMusicRotate(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (RotationSelection)
        {
                // Update the rotation factor based on the beat amplitude.
                rotation = evt.GetValueOfCurveAtTime(sampleTime) * 360.0f;
                Debug.Log("MusicSync: Rotation factor: " + rotation);

                // Update the transform rotation.
                transform.localRotation = Quaternion.Euler(0.0f, 0.0f, rotation);
        }
    }
}
