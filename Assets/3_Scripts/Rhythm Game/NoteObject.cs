using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class NoteObject : MonoBehaviour
{
    public Lane lane;
    public NoteType type;

    public float beatHitTime;
    public float spawnTime;

    public float totalTime;
    private float timer;

    public int tapPosition;
    public int holdToPosition;

    private Vector3 startPosition;
    private Vector3 endPosition;

    //private void Update()
    //{
    //    float ratio = timer / totalTime;

    //    timer += Time.deltaTime;

    //    if (timer >= totalTime)
    //    {
    //        timer = 0;
    //        ObjectPooler<NoteObject>.Instance.Release(this);
    //        return;
    //    }

    //    transform.position = Vector3.Lerp(startPosition, endPosition, ratio);
    //}

    public void Process()
    {
        float dist = Vector3.Distance(startPosition, endPosition);

        float time = ((60f / 140f) / 2f) * tapPosition;

        float speed = (dist / time);

        transform.position += speed * -transform.forward * Time.deltaTime;
    }

    public void InitData(Vector3 start, Vector3 end, Transform parent)
    {
        startPosition = start;
        endPosition = end;

        transform.rotation = parent.rotation;

        //timer = spawnTime;
    }

    public void InitData(Vector3 position, Transform parent, Transform endPos)
    {
        transform.position = position;

        startPosition = position;
        endPosition = endPos.position;

        Vector3 direction = (position - endPos.position).normalized;

        transform.rotation = Quaternion.LookRotation(direction);
    }
}