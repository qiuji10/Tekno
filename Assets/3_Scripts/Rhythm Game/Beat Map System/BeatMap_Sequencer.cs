using Cinemachine;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatMap_Sequencer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BeatMap easyBeatmap;
    [SerializeField] private BeatMap hardBeatmap;
    [SerializeField] private AudioSource _audio;
    [SerializeField] private BeatMap_Instantiator generator;

    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CinemachineBasicMultiChannelPerlin noise;

    private void Awake()
    {
        generator.OnNoteSpawn += Generator_OnNoteSpawn;
        noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void OnDestroy()
    {
        generator.OnNoteSpawn -= Generator_OnNoteSpawn;
    }

    [SerializeField] private List<NoteObject> rhythmNotes = new List<NoteObject>();

    private void Generator_OnNoteSpawn(NoteObject note)
    {
        rhythmNotes.Add(note);
    }

    public void Sequencer_PlaceNotes(BeatMap beatmap)
    {
        for (int i = 0; i < beatmap.notes.Count; i++)
        {
            NoteData noteData = beatmap.notes[i];

            if (noteData != null)
            {
                float timeTakenToDistance = ((60f / beatmap.bpm) / ((float)beatmap.division / beatmap.timeSignature.x)) * noteData.tapPosition;

                generator.SpawnNote(noteData, timeTakenToDistance);
            }
        }

        _audio.Play();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            generator.DestroyPool();
            Sequencer_PlaceNotes(easyBeatmap);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            generator.DestroyPool();
            Sequencer_PlaceNotes(hardBeatmap);
        }

        for (int i = 0; i < rhythmNotes.Count; i++)
        {
            NoteObject note = rhythmNotes[i];

            if (note == null)
            {
                rhythmNotes.Remove(note);
                return;
            }

            if (note.type == NoteType.Tap)
            {
                if (!note.SurpassEndPos && note.SurpassStartPos && !note.visualEnabled)
                {
                    note.EnableVisual();
                }
                else if (note.SurpassEndPos)
                {
                    //if (note.visualEnabled)
                    //    note.DisableVisual();
                    //else
                    //    continue;
                }
            }
            else if (note.type == NoteType.Hold)
            {
                //if (note.SurpassEndPos)
                //{
                //    note.DisableVisual();
                //    continue;
                //}
            }
            
            note.Process();
        }

        if (isSurpassing)
        {
            // Increment the hold time while the note is being held
            holdTime += Time.deltaTime;

            // Check if the hold time exceeds the threshold for camera shake
            if (holdTime >= maxHoldTimeForShake)
            {
                if (virtualCamera != null)
                {
                    if (noise != null)
                    {
                        // Calculate the intensity of the shake based on hold time
                        float intensity = Mathf.Clamp01(((holdTime - maxHoldTimeForShake) * shakeThresold) / (holdTime * shakeThresold)); // Adjust 0.5f as needed

                        // Apply the intensity to the noise profile's amplitude
                        noise.m_AmplitudeGain = intensity;
                        noise.m_FrequencyGain = intensity;
                    }
                }
            }
        }
        else
        {
            noise.m_AmplitudeGain = 0;
            noise.m_FrequencyGain = 0;
            holdTime = 0;
        }
    }

    public static bool isSurpassing;
    [SerializeField] float shakeThresold = 0.5f;
    private float holdTime = 0f;
    public float maxHoldTimeForShake = 2f; // Adjust this value as needed
}

