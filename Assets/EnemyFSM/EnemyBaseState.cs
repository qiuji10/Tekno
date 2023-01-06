using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public abstract class EnemyBaseState : MonoBehaviour
{
    public virtual void Construct() { Debug.Log("Enemy is in  " + this.ToString()); } 
    public virtual void Destruct() { }
    public virtual void Transition() { }

    // can modify the return to anything you see fit
    public virtual Vector3 ImplementMovement() 
    {
        Debug.Log("Movement Is not implemented in " + this.ToString());
        return Vector3.zero;
    }

}
