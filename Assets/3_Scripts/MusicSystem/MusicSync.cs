using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class MusicSync : MonoBehaviour
{
    [EventID]
    public string eventID;
    private Transform transform;
    private Rigidbody2D rigidbody2D;
    private float scaleFactor; // Scale
    private float speed; // movement speed 
    private float rotation; //rotation

    private void Awake()
    {
        transform = GetComponent<Transform>(); //get transform from game object
        rigidbody2D = GetComponent<Rigidbody2D>(); // get rigidbody from the game object.

        // Register the MusicSync instance to receive events.
        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicScale);
        //Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicSpeed);
        Koreographer.Instance.RegisterForEventsWithTime(eventID, OnMusicRotate);
    }

    private void OnMusicScale(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        // Ensure the event is valid.
        if (evt == null)
        {
            Debug.LogError("MusicSync: Invalid event ID. Could not find a Koreography event with the specified ID.");
            return;
        }

        // Update the scale factor based on the beat amplitude.
        scaleFactor = evt.GetValueOfCurveAtTime(sampleTime) * 2.0f;
        Debug.Log("MusicSync: Scale factor: " + scaleFactor);

        // Update the transform scale.
        transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }
        private void OnMusicRotate(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        // Update the rotation factor based on the beat amplitude.
        rotation = evt.GetValueOfCurveAtTime(sampleTime) * 360.0f;
        Debug.Log("MusicSync: Rotation factor: " + rotation);

        // Update the transform rotation.
        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, rotation);
    }
}





//private void OnMusicSpeed(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
//{
//    // Update the moving speed based on the music tempo.
//    speed = evt.tempo* Time.deltaTime;

//    // Update the Rigidbody2D velocity.
//    rigidbody2D.velocity = new Vector2(speed, 0);
//}



