using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;

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

    public void Process()
    {
        float dist = Vector3.Distance(startPosition, endPosition);

        float time = ((60f / 140f) / 2f) * tapPosition;

        float speed = (dist / time);

        transform.position += speed * -transform.forward * Time.deltaTime;
    }

    public void InitData(Vector3 start, Vector3 end)
    {
        transform.position = start;

        startPosition = start;
        endPosition = end;

        Vector3 direction = (start - end).normalized;

        transform.rotation = Quaternion.LookRotation(direction);
    }
}