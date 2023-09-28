using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmGameMiniAmp : MonoBehaviour
{
    [SerializeField] ParticleSystem hitParticle;
    [SerializeField] PlayerStatus status;

    private void Awake()
    {
        if (status)
            status.isGlitchy = true;
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Note") || col.CompareTag("LongNote"))
        {
            status.Damage(1, false);
            hitParticle.Play();
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Note") || col.CompareTag("LongNote"))
        {
            hitParticle.Stop();
        }
    }

    public void StopHitEffect()
    {
        hitParticle.Stop();
    }
}
