using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LaneData
{
    public Transform parentLane;
    public Transform startPos;
    public Transform endPos;
}

public class BeatMap_Instantiator : MonoBehaviour
{
    [Header("Note Prefabs")]
    [SerializeField] private NoteObject tapNotePrefab;
    [SerializeField] private NoteObject holdNotePrefab;

    [Header("Notes Settings")]
    [SerializeField, Range(0f, 20f)] private float noteSpeed = 0.8f;

    [Header("Lanes Settings")]
    [SerializeField] private LaneData lane1;
    [SerializeField] private LaneData lane2;
    [SerializeField] private LaneData lane3;
    [SerializeField] private LaneData lane4;

    private ObjectPooler<NoteObject> notesPool = new ObjectPooler<NoteObject>();

    public event Action<NoteObject> OnNoteSpawn;

    private void Awake()
    {
        notesPool.Initialize(tapNotePrefab, 20);
    }

    public void SpawnNote(NoteData noteData, float timeTakenToDistance)
    {
        NoteObject note = null;

        switch (noteData.type)
        {
            case NoteType.Tap: note = notesPool.GetPooledObject(); break;
            case NoteType.Hold: note = Instantiate(holdNotePrefab); break;
        }

        note.tapPosition = noteData.tapPosition;
        note.type = noteData.type;
        note.lane = noteData.lane;

        LaneData lane = new LaneData();

        switch (noteData.lane)
        {
            case Lane.Lane1: lane = lane1; break;
            case Lane.Lane2: lane = lane2; break;
            case Lane.Lane3: lane = lane3; break;
            case Lane.Lane4: lane = lane4; break;
        }

        float noteDistance = noteSpeed * noteData.tapPosition;

        Vector3 notePosition = lane.endPos.position + (lane.startPos.position - lane.endPos.position).normalized * noteDistance;

        float speed = noteDistance / timeTakenToDistance;

        note.InitNoteData(notePosition, lane, speed);

        OnNoteSpawn?.Invoke(note);
    }
}
