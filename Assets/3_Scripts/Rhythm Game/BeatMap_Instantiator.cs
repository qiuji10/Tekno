using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

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

    public void SpawnNote(NoteData data, float timeTaken, float distance)
    {
        NoteObject note = null;

        switch (data.type)
        {
            case NoteType.Tap: note = notesPool.GetPooledObject(); break;
            case NoteType.Hold: note = Instantiate(holdNotePrefab); break;
        }

        note.tapPosition = data.tapPosition;
        note.type = data.type;
        note.lane = data.lane;
        //note.spawnTime = spawnTime;
        //note.beatHitTime = beatHitTime;
        //note.totalTime = beatHitTime - spawnTime;




        switch (data.lane)
        {
            case Lane.Lane1: note.InitData(lane1.startPos.position, lane1.endPos.position, lane1.parentLane); break;
            case Lane.Lane2: note.InitData(lane2.startPos.position, lane2.endPos.position, lane2.parentLane); break;
            case Lane.Lane3: note.InitData(lane3.startPos.position, lane3.endPos.position, lane3.parentLane); break;
            case Lane.Lane4: note.InitData(lane4.startPos.position, lane4.endPos.position, lane4.parentLane); break;
        }
    }

    public void SpawnNote(NoteData noteData, float position)
    {
        float distance = 60f;
        float distancePerDiv = distance / 17f;

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

        float noteDistance = distancePerDiv * position;

        Vector3 notePosition = lane.endPos.position + (lane.startPos.position - lane.endPos.position).normalized * noteDistance;

        note.InitData(notePosition, lane.parentLane, lane.endPos);

        OnNoteSpawn?.Invoke(note);
    }
}
