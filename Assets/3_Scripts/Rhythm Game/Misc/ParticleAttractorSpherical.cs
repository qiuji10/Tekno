using NaughtyAttributes;
using System.Collections;
using UnityEngine;
[RequireComponent(typeof(ParticleSystem))]
public class ParticleAttractorSpherical : MonoBehaviour
{
    ParticleSystem ps;
    ParticleSystem.Particle[] m_Particles;
    public BossHealth[] targets;
    public Vector3 offset = new Vector3 (0, 2.71f, 0);
    public float speed = 5f;
    int numParticlesAlive, random;
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }
    void Update()
    {
        m_Particles = new ParticleSystem.Particle[ps.main.maxParticles];
        numParticlesAlive = ps.GetParticles(m_Particles);
        float step = speed * Time.deltaTime;
        for (int i = 0; i < numParticlesAlive; i++)
        {
            m_Particles[i].position = Vector3.SlerpUnclamped(m_Particles[i].position, targets[random].transform.position + offset, step);
        }
        ps.SetParticles(m_Particles, numParticlesAlive);
    }

    [Button]
    public void Play()
    {
        random = Random.Range(0, targets.Length);
        ps.Play();
    }

    private void OnParticleSystemStopped()
    {
        
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.transform == targets[0])
        {
            targets[0].PlayHit();
        }
        else if (other.transform == targets[1])
        {
            targets[1].PlayHit();
        }
    }
}
