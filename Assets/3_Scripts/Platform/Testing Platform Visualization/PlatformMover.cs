using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    [SerializeField] private List<Transform> points = new List<Transform>();
    [SerializeField] private int beatCountToTrigger = 3, curBeatCount = 1, index;

    private void Awake()
    {
        TempoManager.OnBeat += TempoManager_OnBeat;
    }

    private void OnDestroy()
    {
        TempoManager.OnBeat -= TempoManager_OnBeat;
    }

    private void TempoManager_OnBeat()
    {
        //if (curBeatCount < beatCountToTrigger)
        //{
        //    curBeatCount++;
        //}
        //else if (curBeatCount == beatCountToTrigger)
        //{
        //    StartCoroutine(MoveLogic());
        //    curBeatCount++;
        //}
        //else
        //{
        //    curBeatCount = 1;
        //}

        if (TempoManager.beatCount == 4)
        {
            StartCoroutine(MoveLogic());
        }
    }

    private IEnumerator MoveLogic()
    {
        Vector3 oldPos = transform.position;
        Vector3 newPos = points[index].position;

        float timer = 0f;
        float time = (60f / TempoManager.staticBPM) * 1f;

        while (timer < time)
        {
            float ratio = Mathf.SmoothStep(0f, 1f, timer / time);
            transform.position = Vector3.Lerp(oldPos, newPos, ratio);
            timer += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        index++;

        if (index == points.Count)
            index = 0;

        transform.position = newPos;

        
    }

}
