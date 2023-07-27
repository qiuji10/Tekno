using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatMap_Sequencer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BeatMap beatmap;
    [SerializeField] private AudioSource _audio;
    [SerializeField] private BeatMap_Instantiator generator;

    private void Awake()
    {
        generator.OnNoteSpawn += Generator_OnNoteSpawn;
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

    /// <summary>
    /// Sequencer Main Logic-Runner
    /// </summary>
    /// <returns></returns>
    [Button("Start Play")]
    public void Sequencer_PlaceNotes()
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
            Sequencer_PlaceNotes();
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
    }
}

