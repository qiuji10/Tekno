using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class NoteObject_Hold : NoteObject
{
    public int holdToPosition;

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

    public override void Process()
    {
        transform.position += speed * -transform.forward * Time.deltaTime;
    }

    public override void InitNoteData(Vector3 position, LaneData lane, float speed)
    {
        transform.position = position;
        transform.rotation = Quaternion.LookRotation((lane.startPos.position - lane.endPos.position).normalized);

        startPos = lane.startPos.position;
        endPos = lane.endPos.position;

        this.speed = speed;

        DisableVisual(true);
    }

    public override void EnableVisual()
    {
        if (visualEnabled) return;

        visualEnabled = true;
        _startMesh.enabled = true;
        _endMesh.enabled = true;
        _effect.Play();
    }

    public override void DisableVisual(bool forceDisable = false)
    {
        if (!forceDisable && !visualEnabled) return;

        visualEnabled = false;
        _startMesh.enabled = false;
        _endMesh.enabled = false;
        _effect.Stop();
    }

    private void SetPosition(int index, Vector3 position)
    {
        if (index == 1)
        {
            noteStart.position = position;
            _effect.SetVector3(pos1, position);
        }
        else if (index == 2)
        {
            noteEnd.position = position;
            _effect.SetVector3(pos2, position);
        }
    }
}
