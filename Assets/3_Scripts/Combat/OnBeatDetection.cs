using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnBeatDetection : MonoBehaviour
{
    public float bufferMargin = 0.25f;
    public TempoManager tempoManager;
    private float _lastBeatTime = 0f;
    private bool _hasDetectedInput = false;
    public KeyCode _key = KeyCode.Mouse0;

    void OnEnable()
    {
        TempoManager.OnBeat += OnBeat;
    }

    void OnDisable()
    {
        TempoManager.OnBeat -= OnBeat;
    }

    void Update()
    {
        if (!_hasDetectedInput && Input.GetKeyDown(_key))
        {
            float timeSinceLastBeat = Time.time - _lastBeatTime;
            float margin = TempoManager.BeatsPerMinuteToDelay(tempoManager.BPM) * bufferMargin;
            Debug.Log(margin);
            if (timeSinceLastBeat > margin * 2f)
            {
                Debug.Log($"timeSinceLastBeat:{timeSinceLastBeat}, {margin} <color=red>Input too late</color>");
            }
            else if (timeSinceLastBeat >= margin)
            {
                Debug.Log($"timeSinceLastBeat:{timeSinceLastBeat}, {margin} <color=green>Input on beat</color>");
            }
            else if (timeSinceLastBeat < margin && timeSinceLastBeat > margin / 2f)
            {
                Debug.Log($"timeSinceLastBeat:{timeSinceLastBeat}, {margin} <color=green>Input a bit early</color>");
            }
            else
            {
                Debug.Log($"timeSinceLastBeat:{timeSinceLastBeat}, {margin} <color=yellow>Input too early</color>");
            }

            _hasDetectedInput = true;
        }
    }

    private void OnBeat()
    {
        _lastBeatTime = Time.time;
        _hasDetectedInput = false;
    }
}