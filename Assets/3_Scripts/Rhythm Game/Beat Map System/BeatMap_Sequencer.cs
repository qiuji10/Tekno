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

    private Dictionary<int, NoteData[]> sequence = new Dictionary<int, NoteData[]>();

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
        AssignNotesToSequence(beatmap);

        int currentPosition = 0;

        while (currentPosition <= sequence.Count)
        {
            if (sequence.ContainsKey(currentPosition))
            {   
                NoteData[] notesArray = sequence[currentPosition];
                for (int i = 0; i < notesArray.Length; i++)
                {
                    NoteData noteData = notesArray[i];

                    if (noteData != null)
                    {
                        float timeTakenToDistance = ((60f / beatmap.bpm) / ((float)beatmap.division / beatmap.timeSignature.x)) * noteData.tapPosition;

                        generator.SpawnNote(noteData, timeTakenToDistance);
                    }
                }
            }

            currentPosition++;
        }

        _audio.Play();
    }

    private void Update()
    {
        for (int i = 0; i < rhythmNotes.Count; i++)
        {
            NoteObject note = rhythmNotes[i];

            if (!note.SurpassEndPos && note.SurpassStartPos && !note.visualEnabled)
            {
                note.EnableVisual();
            }
            else if (note.SurpassEndPos)
            {
                note.DisableVisual();
            }
            
            note.Process();
        }
    }

    public void AssignNotesToSequence(BeatMap beatmap)
    {
        sequence.Clear();

        List<int> tapPositions = new List<int>();
        foreach (NoteData note in beatmap.notes)
        {
            if (note.tapPosition > 0)
            {
                tapPositions.Add(note.tapPosition);
            }
        }

        tapPositions.Sort();

        int currentPosition = 1;
        foreach (int tapPosition in tapPositions)
        {
            while (currentPosition < tapPosition)
            {
                AddNullNotesToSequence(currentPosition);
                currentPosition++;
            }

            NoteData note = GetConvertedNoteFromOriginal(beatmap.notes, tapPosition);
            AddNoteToSequence(currentPosition, note);
            currentPosition++;
        }

        // Fill in remaining positions with null notes
        while (currentPosition <= beatmap.timeSignature.x + 1)
        {
            AddNullNotesToSequence(currentPosition);
            currentPosition++;
        }
    }

    /// <summary>
    /// This is a Note class converter
    /// </summary>
    /// <param name="beatmap"></param>
    /// <param name="tapPosition"></param>
    /// <returns></returns>
    private NoteData GetConvertedNoteFromOriginal(List<NoteData> notes, int tapPosition)
    {
        foreach (NoteData note in notes)
        {
            if (note.tapPosition == tapPosition)
            {
                if (note.type == NoteType.Tap)
                {
                    return new Note_Tap()
                    {
                        type = NoteType.Tap,
                        lane = note.lane,
                        tapPosition = note.tapPosition
                    };
                }
                else if (note.type == NoteType.Hold)
                {
                    return new Note_Hold()
                    {
                        type = NoteType.Hold,
                        lane = note.lane,
                        tapPosition = note.tapPosition,
                        holdToPosition = ((Note_Hold)note).holdToPosition
                    };
                }
            }
        }

        // No note found for the given tap position
        return null;
    }

    private void AddNullNotesToSequence(int position)
    {
#nullable enable
        NoteData?[] notesArray = new NoteData?[4];
#nullable disable
        sequence.Add(position, notesArray);
    }

    private void AddNoteToSequence(int position, NoteData note)
    {
        if (!sequence.ContainsKey(position))
        {
#nullable enable
            NoteData?[] notesArray = new NoteData?[4];
#nullable disable
            notesArray[(int)note.lane] = note;
            sequence.Add(position, notesArray);
        }
        else
        {
#nullable enable
            NoteData?[] notesArray = sequence[position];
#nullable disable
            notesArray[(int)note.lane] = note;
            sequence[position] = notesArray;
        }
    }
}

