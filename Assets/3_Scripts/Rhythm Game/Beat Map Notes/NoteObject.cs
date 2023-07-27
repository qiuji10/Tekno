using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    [Header("Basic Info")]
    public Lane lane;
    public NoteType type;
    public bool visualEnabled;
    [SerializeField] protected float delayDisableVisual;

    [Header("Position Data")]
    public int tapPosition;
    
    public virtual bool SurpassStartPos => Vector3.Dot(transform.position - laneStartPos, transform.forward) < 0f;
    public virtual bool SurpassEndPos => Vector3.Dot(transform.position - laneEndPos, transform.forward) < (1f * delayDisableVisual);

    protected float speed;
    protected Vector3 laneStartPos;
    protected Vector3 laneEndPos;

    public virtual void Process()
    {

    }

    public virtual void EnableVisual()
    {

    }

    public virtual void DisableVisual(bool forceDisable = false)
    {

    }

    public virtual void DisableNote(float delay)
    {

    }

    
}