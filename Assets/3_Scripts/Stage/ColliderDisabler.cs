using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderDisabler : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.enabled = false;
            StartCoroutine(EnableCollider(other));
        }
    }

    IEnumerator EnableCollider(Collider collider)
    {
        yield return new WaitForSeconds(1f);
        collider.enabled = true;
    }
}
