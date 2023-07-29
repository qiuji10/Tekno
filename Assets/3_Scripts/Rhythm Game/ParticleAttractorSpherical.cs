using NaughtyAttributes;
using System.Collections;
using UnityEngine;
[RequireComponent(typeof(ParticleSystem))]
public class ParticleAttractorSpherical : MonoBehaviour
{
    ParticleSystem ps;
    ParticleSystem.Particle[] m_Particles;
    public Transform[] targets;
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
            m_Particles[i].position = Vector3.SlerpUnclamped(m_Particles[i].position, targets[random].position, step);
        }
        ps.SetParticles(m_Particles, numParticlesAlive);
    }

    [Button]
    public void Play()
    {
        random = Random.Range(0, targets.Length);
        ps.Play();
    }
}
