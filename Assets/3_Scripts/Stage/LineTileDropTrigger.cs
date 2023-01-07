using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LineTileDropTrigger : MonoBehaviour
{
    [Serializable]
    private class MovingPathData
    {
        public string name;
        public float lerpTime = 10f;
        public Vector3 startPos;
        public Vector3 endPos;
    }

    [SerializeField] private StageManager stageManager;

    [Header("Moving Data")]
    [SerializeField] private int index;
    [SerializeField] private List<MovingPathData> movingData;

    [Header("Moving Settings")]
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform endPos;
    [SerializeField] private float movingTime;

    private float dropDelay = 0.5f, dropTime = 0.7f, liftTime = 1f;

    private Material matColorer;
    private int lastRandom;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out FloorTile floorTile))
        {
            floorTile.dropTimer = dropTime;
            floorTile.liftTimer = liftTime;
            floorTile.SetMat(matColorer);
            floorTile.DropWithAlert(dropDelay);
            StartCoroutine(SwitchDefaultMat(floorTile, stageManager.Mat_default));
        }
    }

    private void RandomMat()
    {
        int rand = Random.Range(0, 3);

        while (rand == lastRandom)
        {
            rand = Random.Range(0, 3);
        }

        switch (rand)
        {
            case 0:
                matColorer = stageManager.Mat_Red;
                break;
            case 1:
                matColorer = stageManager.Mat_Blue;
                break;
            case 2:
                matColorer = stageManager.Mat_Green;
                break;
        }

        lastRandom = rand;
    }

    [Button]
    private void Move()
    {
        movingTime = movingData[index].lerpTime;
        startPos.position = movingData[index].startPos;
        endPos.position = movingData[index].endPos;

        StartCoroutine(Moving_Logic());
    }

    private IEnumerator Moving_Logic()
    {
        RandomMat();
        float timer = 0;

        transform.position = startPos.position;
        transform.LookAt(endPos);

        while (timer < movingTime)
        {    
            transform.position = Vector3.Lerp(startPos.position, endPos.position, timer / movingTime);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator SwitchDefaultMat(FloorTile tile, Material mat)
    {
        yield return new WaitForSeconds(dropTime);
        tile.SetMat(mat);
    }

    
}
