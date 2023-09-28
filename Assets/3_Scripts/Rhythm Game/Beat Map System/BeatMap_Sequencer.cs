using Cinemachine;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatMap_Sequencer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BeatMap mediumBeatmap;
    [SerializeField] private AudioClip mediumAudioClip;
    [SerializeField] private BeatMap hardBeatmap;
    [SerializeField] private AudioClip hardAudioClip;
    [SerializeField] private AudioSource _audio;
    [SerializeField] private BeatMap_Instantiator generator;

    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CinemachineBasicMultiChannelPerlin noise;

    [SerializeField] private UnityEvent OnRhythmGameEnd;

    bool startedPlay;

    private void Awake()
    {
        isSurpassing = false;
        generator.OnNoteSpawn += Generator_OnNoteSpawn;
        noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);

        _audio.clip = mediumAudioClip;
        generator.DestroyPool();
        Sequencer_PlaceNotes(mediumBeatmap);
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

        startedPlay = true;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    _audio.clip = mediumAudioClip;
        //    generator.DestroyPool();
        //    Sequencer_PlaceNotes(mediumBeatmap);
        //}

        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    _audio.clip = hardAudioClip;
        //    generator.DestroyPool();
        //    Sequencer_PlaceNotes(hardBeatmap);
        //}

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

        if (startedPlay)
        {
            if (!_audio.isPlaying)
            {
                OnRhythmGameEnd?.Invoke();

                startedPlay = false;
            }
        }
    }

    public static bool isSurpassing;
    [SerializeField] float shakeThresold = 0.5f;
    private float holdTime = 0f;
    public float maxHoldTimeForShake = 2f; // Adjust this value as needed
}

