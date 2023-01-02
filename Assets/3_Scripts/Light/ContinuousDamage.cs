using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousDamage : MonoBehaviour
{
    private MaterialModifier modifier;

    private void Awake()
    {
        modifier = FindObjectOfType<MaterialModifier>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            modifier.StopCoroutines();
            modifier.GlitchyEffectOn();
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            modifier.StopCoroutines();
            modifier.GlitchyEffectOff();
        }
    }

    //private void OnTriggerStay(Collider col)
    //{
    //    if (col.CompareTag("Player"))
    //    {
    //        Debug.Log("Damage");
    //    }
    //}
}
