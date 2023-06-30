using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    public Lane lane;
    public NoteType type;

    public int tapPosition;
    public int holdToPosition;

    public bool SurpassEndPos => Vector3.Dot(transform.position - endPos, transform.forward) < 0f;
    public bool SurpassStartPos => Vector3.Dot(transform.position - startPos, transform.forward) < 0f;
    public bool visualEnabled;

    private float speed;
    private Vector3 startPos;
    private Vector3 endPos;

    private MeshRenderer _mesh;

    private void Awake()
    {
        _mesh = GetComponent<MeshRenderer>();
    }

    public void Process()
    {
        transform.position += speed * -transform.forward * Time.deltaTime;
    }

    public void InitNoteData(Vector3 position, LaneData lane, float speed)
    {
        transform.position = position;
        transform.rotation = Quaternion.LookRotation((lane.startPos.position - lane.endPos.position).normalized);

        startPos = lane.startPos.position;
        endPos = lane.endPos.position;

        this.speed = speed;

        DisableVisual(true);
    }

    public void EnableVisual()
    {
        if (visualEnabled) return;

        visualEnabled = true;
        _mesh.enabled = true;
    }

    public void DisableVisual(bool forceDisable = false)
    {
        if (!forceDisable && !visualEnabled) return;

        _mesh.enabled = false;
        _mesh.enabled = false;
    }
}