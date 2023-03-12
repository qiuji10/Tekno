using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class MusicSync : MonoBehaviour
{
    [Header("Selected Beat Data")]
    [EventID]
    public string eventID;
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
    private void Awake()
    {
        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicScale);
        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicRotate);
    }

    private void OnMusicScale(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
       if (ScaleSelection)
        {
            scale.x = ScaleX ? evt.GetValueOfCurveAtTime(sampleTime) * 2.0f * scaleMod.x : scale.x;
            scale.y = ScaleY ? evt.GetValueOfCurveAtTime(sampleTime) * 2.0f * scaleMod.y : scale.y;
            scale.z = ScaleZ ? evt.GetValueOfCurveAtTime(sampleTime) * 2.0f * scaleMod.z : scale.z;

            transform.localScale = scale;
        }
      
    }
        private void OnMusicRotate(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (RotationSelection)
        {
            

            float x = RotateX ? evt.GetValueOfCurveAtTime(sampleTime) * 360.0f: 0.0f;
            float y = RotateY ? evt.GetValueOfCurveAtTime(sampleTime) * 360.0f : 0.0f;
            float z = RotateZ ? evt.GetValueOfCurveAtTime(sampleTime) * 360.0f : 0.0f;

            transform.localRotation = Quaternion.Euler(x, y, z);
        }
    }
}
