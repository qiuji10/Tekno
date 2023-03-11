using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialWallsOnBeat : MonoBehaviour
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

    private void Awake()
    {
        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicReact);
    }

    private void OnMusicReact(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (ScaleSelection)
        {
            scale.x = ScaleX ? evt.GetValueOfCurveAtTime(sampleTime) * 2.0f : scale.x;
            scale.y = ScaleY ? evt.GetValueOfCurveAtTime(sampleTime) * 2.0f : scale.y;
            scale.z = ScaleZ ? evt.GetValueOfCurveAtTime(sampleTime) * 2.0f : scale.z;

            transform.localScale = scale;
        }

    }
   
}
