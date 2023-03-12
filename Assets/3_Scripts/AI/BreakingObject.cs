using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BreakingObject : MonoBehaviour
{
    private List<Rigidbody> rbs = new List<Rigidbody>();
    private List<Collider> colliders = new List<Collider>();

    private void Awake()
    {
        rbs = GetComponentsInChildren<Rigidbody>().ToList();
        colliders = GetComponentsInChildren<Collider>().ToList();
    }

    public void BreakObjects()
    {
        foreach (var rb in rbs)
        {
            rb.isKinematic = false;
            rb.transform.SetParent(null);
        }

        foreach (var collider in colliders)
        {
            collider.enabled = true;
        }
    }
}
