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
        transform.LeanMoveLocal(points[beatCount].localPosition, TempoManager.GetTimeToBeatCount(1));
    }
}
