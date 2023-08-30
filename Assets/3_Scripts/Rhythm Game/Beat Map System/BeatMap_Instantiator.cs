using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LaneData
{
    [Header("References")]
    public Transform parentLane;
    public Transform startPos;
    public Transform endPos;

    [Header("Color amd Materials")]
    public Material material;
    [ColorUsage(false, true)] public Color baseColor;
    [ColorUsage(false, true)] public Color secondaryColor;
    public Gradient rangeColor;
}

public class BeatMap_Instantiator : MonoBehaviour
{
    [Header("Note Prefabs")]
    [SerializeField] private NoteObject_Tap tapNotePrefab;
    [SerializeField] private NoteObject_Hold holdNotePrefab;

    [Header("Notes Settings")]
    [SerializeField, Range(0f, 20f)] private float noteSpeed = 0.8f;

    [Header("Lanes Settings")]
    [SerializeField] private LaneData lane1;
    [SerializeField] private LaneData lane2;
    [SerializeField] private LaneData lane3;
    [SerializeField] private LaneData lane4;

    private ObjectPooler<NoteObject_Tap> tapNotesPool = new ObjectPooler<NoteObject_Tap>();
    private ObjectPooler<NoteObject_Hold> holdNotesPool = new ObjectPooler<NoteObject_Hold>();

    public event Action<NoteObject> OnNoteSpawn;

    private void Awake()
    {
        tapNotesPool.Initialize(tapNotePrefab, 20);
        holdNotesPool.Initialize(holdNotePrefab, 20);
    }

    public void DestroyPool()
    {
        if (tapNotesPool != null && holdNotesPool != null)
        {
            tapNotesPool.DestroyAllPooledObjects();
            holdNotesPool.DestroyAllPooledObjects();
        }
    }

    public void SpawnNote(NoteData noteData, float timeTakenToDistance)
    {
        NoteObject note = null;

        switch (noteData.type)
        {
            case NoteType.Tap: note = tapNotesPool.GetPooledObject(); break;
            case NoteType.Hold: note = holdNotesPool.GetPooledObject(); break;
        }

        note.type = noteData.type;
        note.lane = noteData.lane;

        note.tapPosition = noteData.tapPosition;

        if (note is NoteObject_Hold)
        {
            (note as NoteObject_Hold).holdToPosition = noteData.holdToPosition;
        }

        LaneData lane = GetLane(note.lane);

        note.baseColor = lane.baseColor;
        note.rangeColor = lane.rangeColor;

        float noteDistance = noteSpeed * noteData.tapPosition;

        Vector3 noteTapPosition = lane.endPos.position + (lane.startPos.position - lane.endPos.position).normalized * noteDistance;

        float speed = noteDistance / timeTakenToDistance;

        if (note.type == NoteType.Tap)
        {
            (note as NoteObject_Tap).InitNoteData(noteTapPosition, lane, speed);
        }
        else if (note.type == NoteType.Hold)
        {
            Vector3 noteHoldToPosition = lane.endPos.position + (lane.startPos.position - lane.endPos.position).normalized * (noteSpeed * noteData.holdToPosition);
            (note as NoteObject_Hold).InitNoteData(noteTapPosition, noteHoldToPosition, lane, speed);
        }

        OnNoteSpawn?.Invoke(note);
    }

    private LaneData GetLane(Lane laneType)
    {
        LaneData lane = new LaneData();

        switch (laneType)
        {
            case Lane.Lane1: lane = lane1; break;
            case Lane.Lane2: lane = lane2; break;
            case Lane.Lane3: lane = lane3; break;
            case Lane.Lane4: lane = lane4; break;
        }

        return lane;
    }
}
