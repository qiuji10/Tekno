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

    [Header("Moving Data")]
    [SerializeField] private int index;
    [SerializeField] private List<MovingPathData> movingData;

    [Header("Moving Settings")]
    [SerializeField] private Vector3 startPos;
    [SerializeField] private Vector3 endPos;
    [SerializeField] private float movingTime;

    private float dropDelay = 0.5f, dropTime = 0.7f, liftTime = 1f;

    public float DropDelay { get => dropDelay; set => value = dropDelay; }
    public float DropTime { get => dropTime; }

    public Material matColorer { get; set; }
    public Material finalMat { get; set; }
    public Collider trigger { get; set; }
    private int lastRandom;

    private void Awake()
    {
        trigger = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out FloorTile floorTile))
        {
            floorTile.dropTimer = dropTime;
            floorTile.liftTimer = liftTime;
            floorTile.SetMat(matColorer);
            floorTile.DropWithAlert(dropDelay);
            StartCoroutine(SwitchDefaultMat(floorTile, finalMat));
        }
    }

    //private void RandomMat()
    //{
    //    int rand = Random.Range(0, 3);

    //    while (rand == lastRandom)
    //    {
    //        rand = Random.Range(0, 3);
    //    }

    //    switch (rand)
    //    {
    //        case 0:
    //            matColorer = stageManager.Mat_Red;
    //            break;
    //        case 1:
    //            matColorer = stageManager.Mat_Blue;
    //            break;
    //        case 2:
    //            matColorer = stageManager.Mat_Green;
    //            break;
    //    }

    //    lastRandom = rand;
    //}

    public void Move(int index)
    {
        movingTime = movingData[index].lerpTime;
        startPos = movingData[index].startPos;
        endPos = movingData[index].endPos;

        StartCoroutine(Moving_Logic());
    }

    private IEnumerator Moving_Logic()
    {
        float timer = 0;

        transform.position = startPos;
        transform.LookAt(endPos);

        while (timer < movingTime)
        {    
            transform.position = Vector3.Lerp(startPos, endPos, timer / movingTime);
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
