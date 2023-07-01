using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    [Header("Basic Info")]
    public Lane lane;
    public NoteType type;
    public bool visualEnabled;

    [Header("Position Data")]
    public int tapPosition;
    
    public virtual bool SurpassEndPos => Vector3.Dot(transform.position - endPos, transform.forward) < 0f;
    public virtual bool SurpassStartPos => Vector3.Dot(transform.position - startPos, transform.forward) < 0f;

    protected float speed;
    protected Vector3 startPos;
    protected Vector3 endPos;

    public virtual void Process()
    {

    }

    public virtual void InitNoteData(Vector3 position, LaneData lane, float speed)
    {

    }

    public virtual void EnableVisual()
    {

    }

    public virtual void DisableVisual(bool forceDisable = false)
    {

    }
}