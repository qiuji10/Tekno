using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject_Tap : NoteObject
{
    private MeshRenderer _mesh;
    private Coroutine disableTask;
    private ParticleSystem particle;
    private Light pointLight;

    [SerializeField] AudioData sfx;
    private void Awake()
    {
        _mesh = GetComponent<MeshRenderer>();
        particle = GetComponentInChildren<ParticleSystem>();
        pointLight = GetComponentInChildren<Light>();
    }

    public void InitNoteData(Vector3 position, LaneData lane, float speed)
    {
        transform.position = position;
        transform.rotation = Quaternion.LookRotation((lane.startPos.position - lane.endPos.position).normalized);

        laneStartPos = lane.startPos.position;
        laneEndPos = lane.endPos.position;

        // Access the main module of the ParticleSystem
        ParticleSystem.MainModule mainModule = particle.main;
        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = particle.colorOverLifetime;

        // Change the start color to a new color (e.g., red)
        _mesh.material = lane.material;
        mainModule.startColor = new ParticleSystem.MinMaxGradient(baseColor);
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(rangeColor);

        this.speed = speed;

        DisableVisual(true);
    }

    public override void Process()
    {
        transform.position += speed * -transform.forward * Time.deltaTime;

        if (SurpassEndPos && disableTask == null)
        {
            DisableNote(0.25f);
        }
    }

    public override void EnableVisual()
    {
        if (visualEnabled) return;

        visualEnabled = true;
        _mesh.enabled = true;
        pointLight.enabled = true;
        particle.Play();
    }

    public override void DisableVisual(bool forceDisable = false)
    {
        if (!forceDisable && !visualEnabled) return;

        visualEnabled = false;
        _mesh.enabled = false;
        pointLight.enabled = false;
        particle.Stop();
    }

    public override void DisableNote(float delay)
    {
        if (gameObject.activeInHierarchy)
            disableTask = StartCoroutine(DisableNote_Delay(delay));
    }

    private IEnumerator DisableNote_Delay(float delay)
    {
        yield return new WaitForSeconds(delay);
        BeatMap_Input.CallTapNoteEnd(lane);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BeatPoint"))
        {
            StopAllCoroutines();
            particle.transform.parent = null;
            particle.Stop();
            pointLight.gameObject.SetActive(false);
            BeatMap_Input.CallSuccess(lane);
            gameObject.SetActive(false);
        }
    }
}
