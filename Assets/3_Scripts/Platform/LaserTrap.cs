using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTrap : MonoBehaviour
{
    public GameObject[] LaserObjects;
    public Transform[] StartPoints;
    public Transform[] EndPoints;

    private LineRenderer[] laserRenderers;
    private ParticleSystem[] particleSystems;
    private float beatInterval;
    private float timer;

    void Start()
    {
        laserRenderers = new LineRenderer[LaserObjects.Length];
        particleSystems = new ParticleSystem[LaserObjects.Length];


        for (int i = 0; i < LaserObjects.Length; i++)
        {
            laserRenderers[i] = LaserObjects[i].transform.GetChild(0).GetComponent<LineRenderer>();
            particleSystems[i] = LaserObjects[i].transform.GetChild(1).GetComponent<ParticleSystem>();
        }

        beatInterval = (60f / 140f)*8f; // calculate the interval between beats based on the song's BPM
        timer = beatInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                // enable or disable the LineRenderer on each Laser object based on its current state
                for (int i = 0; i < laserRenderers.Length; i++)
                {
                    LineRenderer laserRenderer = laserRenderers[i];
                    ParticleSystem particleSystem = particleSystems[i];

                    if (laserRenderer.enabled)
                    {
                        laserRenderer.enabled = false;
                        particleSystem.Stop(); // Stop the particle system when the laser is turned off
                    }
                    else
                    {
                        laserRenderer.enabled = true;
                        particleSystem.Play(); // Play the particle system when the laser is turned on
                    }

                    Vector3 startPointPos = StartPoints[i].position;
                    Vector3 endPointPos = EndPoints[i].position;

                    // transform the start and end point positions to the Laser object's local space
                    Vector3 localStartPointPos = LaserObjects[i].transform.InverseTransformPoint(startPointPos);
                    Vector3 localEndPointPos = LaserObjects[i].transform.InverseTransformPoint(endPointPos);

                    // set the LineRenderer's positions based on the local start and end point positions
                    laserRenderer.SetPosition(0, localStartPointPos);
                    laserRenderer.SetPosition(1, localEndPointPos);

                    // set the particle system's start position to the local start point position
                    particleSystem.transform.localPosition = localStartPointPos;
                }

                timer = beatInterval;
            }
        }
    }

    private void OnEnable()
    {
        StanceManager.OnStanceChangeStart += StanceManager_OnStanceChange;
    }

    private void StanceManager_OnStanceChange(Track obj)
    {
        timer = 0;
    }

    private void OnDisable()
    {
        StanceManager.OnStanceChangeStart -= StanceManager_OnStanceChange;
    }
}



