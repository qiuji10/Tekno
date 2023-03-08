using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatScaler : MonoBehaviour
{
    [SerializeField] private Vector3 beatScale;
    [SerializeField] private Vector3 toScale;
    [SerializeField] private float scaleSpeed = 2f;

    private void OnEnable()
    {
        TempoManager.OnBeat += TempoManager_OnBeat;
    }

    private void OnDisable()
    {
        TempoManager.OnBeat -= TempoManager_OnBeat;
    }

    private void TempoManager_OnBeat()
    {
        transform.localScale = beatScale;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, toScale, Time.deltaTime * scaleSpeed);
    }
}
