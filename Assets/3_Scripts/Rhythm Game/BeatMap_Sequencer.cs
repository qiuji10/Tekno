using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [Button]
    public void StartPlay()
    {
        AssignNotesToSequence(beatmap);
        StartCoroutine(Generator());
    }

    /// <summary>
    /// Sequencer Main Logic-Runner
    /// </summary>
    /// <returns></returns>
    private IEnumerator Generator()
    {
        int bpm = beatmap.bpm;
        int division = (int)beatmap.division;
        Vector2Int timeSignature = beatmap.timeSignature;

        float secondPerBeat = 60f / bpm;
        float divPerBeat = division / timeSignature.x;
        float divPerSec = secondPerBeat / divPerBeat;

        int currentPosition = 0;
        int audioStartPosition = GetFirstAvailableTapPosition();

        float delay = divPerSec * audioStartPosition;

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
                        // Process the note, e.g., instantiate game object, trigger event, etc.
                        // Here have 4 lane, so in maximum, it will loop 4 times
                        // Debug.Log("Note generated at position " + currentPosition + ", Lane: " + noteData.lane + ", Type: " + noteData.type);

                        float currentTime = Time.time;

                        //float beatTime = (divPerSec * noteData.tapPosition);

                        float posTime = currentTime + delay;

                        //generator.SpawnNote(noteData, currentTime, posTime);

                        Debug.Log($"currentTime {currentTime} - beatTime {posTime} - totalTime = {posTime - currentTime}");
                    }
                }
            }

            
            yield return new WaitForSeconds(divPerSec);

            currentPosition++;
        }
    }

    [SerializeField] private List<NoteObject> rhythmNotes = new List<NoteObject>();

    private void Generator_OnNoteSpawn(NoteObject note)
    {
        rhythmNotes.Add(note);
    }

    [Button]
    private void GeneratorV2()
    {
        AssignNotesToSequence(beatmap);
        int bpm = beatmap.bpm;
        int division = (int)beatmap.division;
        Vector2Int timeSignature = beatmap.timeSignature;

        float secondPerBeat = 60f / bpm;
        float divPerBeat = division / timeSignature.x;
        float divPerSec = secondPerBeat / divPerBeat;

        int currentPosition = 0;
        int audioStartPosition = GetFirstAvailableTapPosition();

        float delay = divPerSec * audioStartPosition;


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
                        generator.SpawnNote(noteData, noteData.tapPosition);
                    }
                }
            }

            currentPosition++;
        }

        _audio.Play();
        startPlay = true;
    }

    private bool startPlay;

    private void Update()
    {
        for (int i = 0; i < rhythmNotes.Count; i++)
        {
            rhythmNotes[i].Process();
        }
    }

    /// <summary>
    /// This function is for getting the first available beat in beatmap, to determine which position on the map to play the audio
    /// </summary>
    /// <returns></returns>
    private int GetFirstAvailableTapPosition()
    {
        foreach (NoteData[] notesArray in sequence.Values)
        {
            foreach (NoteData note in notesArray)
            {
                if (note != null && note.tapPosition > 0)
                {
                    return note.tapPosition;
                }
            }
        }

        return 0; // Return 0 if no available tap position is found
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
    /// This is a Note class converter, mainly used for json data R/W
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
        NoteData?[] notesArray = new NoteData?[4];
        sequence.Add(position, notesArray);
    }

    private void AddNoteToSequence(int position, NoteData note)
    {
        if (!sequence.ContainsKey(position))
        {
            NoteData?[] notesArray = new NoteData?[4];
            notesArray[(int)note.lane] = note;
            sequence.Add(position, notesArray);
        }
        else
        {
            NoteData?[] notesArray = sequence[position];
            notesArray[(int)note.lane] = note;
            sequence[position] = notesArray;
        }
    }
}
