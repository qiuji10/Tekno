using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatPlatform : MonoBehaviour
{
    [SerializeField] private int beatCount;
    [SerializeField] private List<Transform> points;

    private Vector3 prevPos;

    private void OnEnable()
    {
        TempoManager.OnBeat += TempoManager_OnBeat;
    }

    private void OnDisable()
    {
        TempoManager.OnBeat -= TempoManager_OnBeat;
    }

    private void Start()
    {
        transform.position = points[0].position;
    }

    private void TempoManager_OnBeat()
    {
        beatCount++;

        if (beatCount == points.Count)
        {
            transform.position = points[0].position;
            beatCount = 0;
        }

        ExecuteMove();
    }

    private void ExecuteMove()
    {
        if (points[beatCount].position == prevPos) return;

        prevPos = points[beatCount].position;
        StartCoroutine(MoveLogic());
    }

    private IEnumerator MoveLogic()
    {
        Vector3 oldPos = transform.position;
        Vector3 newPos = points[beatCount].position;

        float timer = 0f;
        float time = (60f / TempoManager.staticBPM) * 1f;

        while (timer < time)
        {
            float ratio = Mathf.SmoothStep(0f, 1f, timer / time);
            transform.position = Vector3.Lerp(oldPos, newPos, ratio);
            timer += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        transform.position = newPos;
    }
}
