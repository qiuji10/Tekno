using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class NoteObject_Hold : NoteObject
{
    public int holdToPosition;

    public override bool SurpassStartPos => Vector3.Dot(noteStart.position - laneStartPos, noteStart.forward) < 0f;
    public override bool SurpassEndPos => Vector3.Dot(noteEnd.position - laneEndPos, noteEnd.forward) < 0f; //not working currently

    public bool FirstNoteSurpassEndPos => Vector3.Dot(noteStart.position - laneEndPos, noteStart.forward) < 0f;
    public bool SecondNoteSurpassStartPos => Vector3.Dot(noteEnd.position - laneStartPos, noteEnd.forward) < 0f;

    [Header("Extra References")]
    [SerializeField] private Transform noteStart;
    [SerializeField] private Transform noteEnd;

    [Header("Visual Effects")]
    [SerializeField] private VisualEffect _effect;

    private int pos1, pos2;
    private MeshRenderer _startMesh, _endMesh;

    private void Awake()
    {
        _startMesh = noteStart.GetComponent<MeshRenderer>();
        _endMesh = noteEnd.GetComponent<MeshRenderer>();

        pos1 = Shader.PropertyToID("Pos1");
        pos2 = Shader.PropertyToID("Pos2");
        
        _effect.Stop();
    }

    public void InitNoteData(Vector3 noteStartPos, Vector3 noteEndPos, LaneData lane, float speed)
    {
        SetNotePosition(1, noteStartPos);
        SetNotePosition(2, noteEndPos);

        noteStart.rotation = noteEnd.rotation = Quaternion.LookRotation((lane.startPos.position - lane.endPos.position).normalized);

        laneStartPos = lane.startPos.position;
        laneEndPos = lane.endPos.position;

        this.speed = speed;

        DisableVisual(true);
    }

    public override void Process()
    {
        Vector3 velocity = speed * -noteStart.forward * Time.deltaTime;

        if (!SurpassEndPos)
        {
            SetVfxPosition(1, noteStart.position + velocity);
            SetVfxPosition(2, noteEnd.position + velocity);

            SetNotePosition(1, noteStart.position + velocity);
            SetNotePosition(2, noteEnd.position + velocity);

            ToggleNoteMesh(1, false);
            ToggleNoteMesh(1, false);
        }

        if (SurpassStartPos && !FirstNoteSurpassEndPos)
        {
            ToggleNoteMesh(1, true);
        }

        if (FirstNoteSurpassEndPos && !SurpassEndPos)
        {
            ToggleNoteMesh(1, false);
            SetVfxPosition(1, laneEndPos);
        }
        else if (SurpassStartPos && !SurpassEndPos)
        {
            ToggleVFX(true);
            ToggleNoteMesh(2, true);
            SetVfxPosition(2, laneStartPos);
        }
        else if (SurpassEndPos)
        {
            DisableVisual(true);
        }

        
    }

    public override void EnableVisual()
    {
        if (visualEnabled) return;

        visualEnabled = true;
        _startMesh.enabled = true;
        _endMesh.enabled = true;
        ToggleVFX(true);
    }

    public override void DisableVisual(bool forceDisable = false)
    {
        if (!forceDisable && !visualEnabled) return;

        visualEnabled = false;
        _startMesh.enabled = false;
        _endMesh.enabled = false;
        ToggleVFX(false);
    }

    public void ToggleNoteMesh(int index, bool isOn)
    {
        if (index == 1)
        {
            _startMesh.enabled = isOn;
        }
        else
        {
            _endMesh.enabled = isOn;
        }
    }

    public void ToggleVFX(bool isOn)
    {
        if (isOn)
        {
            _effect.enabled = true;
            _effect.Play();
        }
        else
            _effect.enabled = false;
    }

    private void SetNotePosition(int index, Vector3 position)
    {
        if (index == 1)
        {
            noteStart.position = position;
        }
        else if (index == 2)
        {
            noteEnd.position = position;
        }
    }

    private void SetVfxPosition(int index, Vector3 position)
    {
        if (index == 1)
        {
            _effect.SetVector3(pos1, position);
        }
        else if (index == 2)
        {
            _effect.SetVector3(pos2, position);
        }
    }
}
