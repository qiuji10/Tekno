using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmGameMiniAmp : MonoBehaviour
{
    [SerializeField] ParticleSystem hitParticle;

    private void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Note"))
        {
            hitParticle.Play();
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Note"))
        {
            hitParticle.Stop();
        }
    }

    public void StopHitEffect()
    {
        hitParticle.Stop();
    }
}
