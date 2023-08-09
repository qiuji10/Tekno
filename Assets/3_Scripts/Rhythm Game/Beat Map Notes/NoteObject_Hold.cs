using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class NoteObject_Hold : NoteObject
{
    public int holdToPosition;

    public override bool SurpassStartPos => Vector3.Dot(noteStart.position - laneStartPos, noteStart.forward) < 0f;
    public override bool SurpassEndPos => Vector3.Dot(noteEnd.position - laneEndPos, noteEnd.forward) < (1f * delayDisableVisual);

    public bool FirstNoteSurpassEndPos => Vector3.Dot(noteStart.position - laneEndPos, noteStart.forward) < (1f * delayDisableVisual);
    public bool SecondNoteSurpassStartPos => Vector3.Dot(noteEnd.position - laneStartPos, noteEnd.forward) < (1f * delayDisableVisual);

    public float percentage => 1 - (Vector3.Distance(noteEnd.position, laneEndPos) / Vector3.Distance(laneStartPos, laneEndPos));

    [Header("Extra References")]
    [SerializeField] private Transform noteStart;
    [SerializeField] private Transform noteEnd;
    [SerializeField] private BoxCollider _col;

    [Header("Visual Effects")]
    [SerializeField] private VisualEffect _effect;

    private int pos1, pos2, col1, col2, col3;
    private MeshRenderer _startMesh, _endMesh;

    private void Awake()
    {
        _startMesh = noteStart.GetComponent<MeshRenderer>();
        _endMesh = noteEnd.GetComponent<MeshRenderer>();

        pos1 = Shader.PropertyToID("Pos1");
        pos2 = Shader.PropertyToID("Pos2");
        col1 = Shader.PropertyToID("Color1");
        col2 = Shader.PropertyToID("Color2");
        col3 = Shader.PropertyToID("Color3");


        _effect.Stop();
    }

    public void InitNoteData(Vector3 noteStartPos, Vector3 noteEndPos, LaneData lane, float speed)
    {
        SetNotePosition(noteStart, noteStartPos);
        SetNotePosition(noteEnd, noteEndPos);

        // Set the rotation of _col.gameObject to match the rotation of the notes
        _col.transform.rotation = Quaternion.LookRotation((lane.startPos.position - lane.endPos.position).normalized);

        _effect.SetVector4(col1, secondaryColor);
        _effect.SetVector4(col2, baseColor);
        _effect.SetVector4(col3, baseColor);
        _startMesh.material = lane.material;
        _endMesh.material = lane.material;

        // Set the position of _col.gameObject to be in between the start and end notes
        _col.transform.position = (noteStartPos + noteEndPos) / 2f;

        // Set _col.size.z to be the distance between the start and end notes
        _col.size = new Vector3(_col.size.x, _col.size.y, Vector3.Distance(noteStartPos, noteEndPos));

        noteStart.rotation = noteEnd.rotation = Quaternion.LookRotation((lane.startPos.position - lane.endPos.position).normalized);

        laneStartPos = lane.startPos.position;
        laneEndPos = lane.endPos.position;

        this.speed = speed;

        DisableVisual(true);
    }

    public override void Process()
    {
        if (!visualEnabled && SurpassEndPos)
            return;

        Vector3 velocity = speed * -noteStart.forward * Time.deltaTime;

        if (!SurpassEndPos)
        {
            SetVfxPosition(1, noteStart.position + velocity);
            SetVfxPosition(2, noteEnd.position + velocity);

            SetNotePosition(noteStart, noteStart.position + velocity);
            SetNotePosition(noteEnd, noteEnd.position + velocity);
            SetNotePosition(_col.transform, _col.transform.position + velocity);

            ToggleNoteMesh(1, false);
            ToggleNoteMesh(1, false);
        }

        if (SurpassStartPos && !FirstNoteSurpassEndPos)
        {
            ToggleNoteMesh(1, true);

            SetVfxPosition(1, noteStart.position);
        }

        if (SecondNoteSurpassStartPos && !SurpassEndPos)
        {
            ToggleNoteMesh(2, true);
            SetVfxPosition(2, noteEnd.position);
        }

        if (FirstNoteSurpassEndPos && !SurpassEndPos)
        {
            BeatMap_Sequencer.isSurpassing = true;
            BeatMap_Input.inputData[lane] = this;

            ToggleNoteMesh(1, false);
            
            if (_col.gameObject.activeInHierarchy)
            {
                SetVfxPosition(1, laneEndPos + new Vector3(0,0,-6));
            }
            else
            {
                SetVfxPosition(1, laneEndPos);
            }

            if (!SecondNoteSurpassStartPos)
            {
                SetVfxPosition(2, laneStartPos);
            }
            else
            {
                SetVfxPosition(2, noteEnd.position);
            }

            //shake screen
        }
        else if (SurpassStartPos && !SurpassEndPos && !SecondNoteSurpassStartPos)
        {
            ToggleVFX(true);
            SetVfxPosition(2, laneStartPos);
        }
        else if (SurpassEndPos)
        {
            BeatMap_Sequencer.isSurpassing = false;
            BeatMap_Input.inputData[lane] = null;
            BeatMap_Input.CallLongNoteEnd(lane);
            DisableVisual(true);
            gameObject.SetActive(false);
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

    private void SetNotePosition(Transform movingTransform, Vector3 position)
    {
        movingTransform.position = position;
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

    public void ToggleCollider(bool isOn)
    {
        _col.gameObject.SetActive(isOn);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BeatPoint") && BeatMap_Input.inputData[lane] != this)
        {
            BeatMap_Input.inputData[lane] = this;
        }
    }
}
