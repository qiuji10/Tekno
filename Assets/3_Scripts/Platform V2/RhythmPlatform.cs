using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using AYellowpaper.SerializedCollections;

[System.Serializable]
public class PlatformIndex
{
    public Transform transform;
    public int index;
}

public class RhythmPlatform : MonoBehaviour
{
    [SerializeField] List<Transform> points = new();
    [SerializeField] List<PlatformIndex> platforms = new();

    private void OnEnable()
    {
        TempoManager.OnBeat += OnBeat;
    }

    private void OnDisable()
    {
        TempoManager.OnBeat -= OnBeat;
    }

    private void OnBeat()
    {
        for (int i = 0; i < platforms.Count; i++)
        {
            int oldIndex = platforms[i].index;
            int newIndex = oldIndex + 1;

            if (newIndex >= points.Count)
                newIndex = 0;

            platforms[i].index = newIndex;

            Vector3 oldPos = points[oldIndex].position;
            Vector3 newPos = points[newIndex].position;

            StartCoroutine(MoveLogic(platforms[i].transform, oldPos, newPos));
        }
    }

    private IEnumerator MoveLogic(Transform transform, Vector3 oldPos, Vector3 newPos)
    {
        float timer = 0f;
        float time = (60f / TempoManager.staticBPM);

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
