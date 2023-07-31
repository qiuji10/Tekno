using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class MusicSequence : MonoBehaviour
{
    [Header("Beat Settings")]
    [EventID]
    public string eventID;
    public GameObject[] sequence;
    private Vector3[] originalScales;
    private Quaternion[] originalRotations;
    private Vector3[] originalPositions;
    private int currentBeat;

    // Koreography Sync with Stance Manager
    private Track track;
    public static Track currentTrack;

    public Material normalMat;
    public Material houseMaterial;
    public Material technoMaterial;
    public Material electronicMaterial;

    [Header("Scale Settings")]
    public bool useScaling;
    public bool scaleX;
    public bool scaleY;
    public bool scaleZ;
    public Vector3 scaleMod = Vector3.one;

    [Header("Rotate Settings")]
    public bool useRotation;
    public bool rotateX;
    public bool rotateY;
    public bool rotateZ;
    public Vector3 rotationMod = Vector3.zero;

    [Header("Move Settings")]
    public bool useMovement;
    public bool moveX;
    public bool moveY;
    public bool moveZ;
    public Vector3 moveMod = Vector3.zero;

    [Header("Reset Settings")]
    public bool resetOnSequenceComplete = true;
    public float resetDelay = 2.0f;
    private bool[] objectModified;
    private int modifiedObjectCount;

    private void OnDisable()
    {
        StanceManager.OnStanceChangeStart -= StanceManager_OnStanceChange;
    }

    private void Awake()
    {
        StanceManager.OnStanceChangeStart += StanceManager_OnStanceChange;
        // Set the track field to the current track
        //StanceManager_OnStanceChange(StanceManager.curTrack);
        track = currentTrack;

        // Initialize the arrays to store original properties
        originalScales = new Vector3[sequence.Length];
        originalRotations = new Quaternion[sequence.Length];
        originalPositions = new Vector3[sequence.Length];
        objectModified = new bool[sequence.Length];

        for (int i = 0; i < sequence.Length; i++)
        {
            originalScales[i] = sequence[i].transform.localScale;
            originalRotations[i] = sequence[i].transform.localRotation;
            originalPositions[i] = sequence[i].transform.position;
        }
    }

    private void Start()
    {
        StanceManager_OnStanceChange(StanceManager.curTrack);
    }

    private void StanceManager_OnStanceChange(Track obj)
    {
        switch (obj.genre)
        {
            case Genre.House:
                eventID = "120_House_MovingCar";
                break;
            case Genre.Techno:
                eventID = "140_Techno_MovingCar";
                break;
            case Genre.Electronic:
                eventID = "160_Electro_MovingCar";
                break;
            default:
                eventID = "140_Tekno_MovingCar";
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

            // Apply scaling if enabled
            if (useScaling && !objectModified[currentBeat])
            {
                Vector3 scale = originalScales[currentBeat];
                scale.x *= scaleX ? scaleMod.x : 1.0f;
                scale.y *= scaleY ? scaleMod.y : 1.0f;
                scale.z *= scaleZ ? scaleMod.z : 1.0f;
                sequence[currentBeat].transform.localScale = scale;

                // Mark the object as modified
                objectModified[currentBeat] = true;

                // Increment the count of modified objects
                modifiedObjectCount++;

                // If all objects have been modified, start the reset coroutine
                if (modifiedObjectCount == sequence.Length)
                {
                    StartCoroutine(ResetModifiedObjectsAfterDelay(resetDelay));
                }
            }

            // Apply rotation if enabled
            if (useRotation && !objectModified[currentBeat])
            {
                Quaternion rotation = originalRotations[currentBeat];
                float x = rotateX ? rotationMod.x : 0.0f;
                float y = rotateY ? rotationMod.y : 0.0f;
                float z = rotateZ ? rotationMod.z : 0.0f;
                rotation *= Quaternion.Euler(x, y, z);
                sequence[currentBeat].transform.localRotation = rotation;
                // Mark the object as modified
                objectModified[currentBeat] = true;

                // Increment the count of modified objects
                modifiedObjectCount++;

                // If all objects have been modified, start the reset coroutine
                if (modifiedObjectCount == sequence.Length)
                {
                    StartCoroutine(ResetModifiedObjectsAfterDelay(resetDelay));
                }
            }

            // Apply movement if enabled
            if (useMovement && !objectModified[currentBeat])
            {
                Vector3 startPosition = originalPositions[currentBeat];
                Vector3 targetPosition = sequence[currentBeat].transform.position;
                targetPosition.x += moveX ? moveMod.x : 0.0f;
                targetPosition.y += moveY ? moveMod.y : 0.0f;
                targetPosition.z += moveZ ? moveMod.z : 0.0f;

                StartCoroutine(MoveObjectSmoothly(sequence[currentBeat].transform, startPosition, targetPosition));

                // Mark the object as modified
                objectModified[currentBeat] = true;

                // Increment the count of modified objects
                modifiedObjectCount++;

                // If all objects have been modified, start the reset coroutine
                if (modifiedObjectCount == sequence.Length)
                {
                    StartCoroutine(ResetModifiedObjectsAfterDelay(resetDelay));
                }
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

    private IEnumerator ResetModifiedObjectsAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Reset only the modified objects to their original position, scale, and rotation
        for (int i = 0; i < sequence.Length; i++)
        {
            if (objectModified[i])
            {
                sequence[i].transform.localScale = originalScales[i];
                sequence[i].transform.localRotation = originalRotations[i];
                sequence[i].transform.position = originalPositions[i];
                objectModified[i] = false; // Reset the modified flag
            }
        }

        // Reset the modifiedObjectCount to 0
        modifiedObjectCount = 0;
    }

    private IEnumerator MoveObjectSmoothly(Transform objectToMove, Vector3 startPosition, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float duration = 0.1f; // Adjust this value to control the movement speed.

        while (elapsedTime < duration)
        {
            objectToMove.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the object reaches the exact target position
        objectToMove.position = targetPosition;
    }

    private void OnDestroy()
    {
        if (Koreographer.Instance != null)
        {
            Koreographer.Instance.UnregisterForAllEvents(this);
        }
    }
}
