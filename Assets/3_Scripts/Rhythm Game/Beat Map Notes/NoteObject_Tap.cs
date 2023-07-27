using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject_Tap : NoteObject
{
    private MeshRenderer _mesh;
    private Coroutine disableTask;

    private void Awake()
    {
        _mesh = GetComponent<MeshRenderer>();
    }

    public void InitNoteData(Vector3 position, LaneData lane, float speed)
    {
        transform.position = position;
        transform.rotation = Quaternion.LookRotation((lane.startPos.position - lane.endPos.position).normalized);

        laneStartPos = lane.startPos.position;
        laneEndPos = lane.endPos.position;

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
    }

    public override void DisableVisual(bool forceDisable = false)
    {
        if (!forceDisable && !visualEnabled) return;

        visualEnabled = false;
        _mesh.enabled = false;
    }

    public override void DisableNote(float delay)
    {
        if (gameObject.activeInHierarchy)
            disableTask = StartCoroutine(DisableNote_Delay(delay));
    }

    private IEnumerator DisableNote_Delay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BeatPoint"))
        {
            Debug.Log("HI");
            StopAllCoroutines();
            gameObject.SetActive(false);
        }
    }
}
