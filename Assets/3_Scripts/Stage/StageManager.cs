using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;

public enum StageState { Shuffle, Drop }
public enum BossColor { Red, Blue, Green }

public class StageManager : MonoBehaviour
{
    [Header("Floor Materials")]
    [SerializeField] private Material Mat_Red;
    [SerializeField] private Material Mat_Blue;
    [SerializeField] private Material Mat_Green;

    [Header("Floor Tiles")]
    [SerializeField] private List<FloorTile> floorList;
    private List<FloorTile> redFloorList;
    private List<FloorTile> blueFloorList;
    private List<FloorTile> greenFloorList;

    [Header("Settings")]
    [SerializeField] private float shufflePerSec = 3f;

    private bool shuffle;
    private float _timer;
    private float _nextShuffleTime;

    private void Start()
    {
        ShuffleFloor();
        shuffle = true;
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (shuffle)
        {
            if (_timer > _nextShuffleTime)
            {
                ShuffleFloor();
                _nextShuffleTime = Time.time + shufflePerSec;
            }
        }
    }

    private void ShuffleFloor()
    {
        // Shuffle the floorList
        floorList = floorList.OrderBy(x => Random.value).ToList();

        // Divide the list into three equal parts
        int chunkSize = floorList.Count / 3;
        int remainder = floorList.Count % 3;

        int redChunkSize = chunkSize + (remainder > 0 ? 1 : 0);
        int blueChunkSize = chunkSize;
        int greenChunkSize = chunkSize;

        redFloorList = floorList.GetRange(0, redChunkSize);
        blueFloorList = floorList.GetRange(redChunkSize, blueChunkSize);
        greenFloorList = floorList.GetRange(redChunkSize + blueChunkSize, greenChunkSize);
        //Debug.Log($"total: {redFloorList.Count + blueFloorList.Count + greenFloorList.Count} red: {redFloorList.Count}, blue: {blueFloorList.Count}, green: {greenFloorList.Count}");
        /*
        //with specific ratio on specific type
        int redChunkSize = floorList.Count * 2 / 10;
        int remainder = floorList.Count % 10;

        int blueChunkSize = (floorList.Count - redChunkSize) / 2;
        int greenChunkSize = floorList.Count - redChunkSize - blueChunkSize;

        // Adjust the sizes of the second and third chunks if necessary
        if (remainder > 2)
        {
            blueChunkSize++;
            greenChunkSize++;
        }
        else if (remainder > 0)
        {
            greenChunkSize++;
        }
        */

        SetFloorsMaterial(redFloorList, Mat_Red);
        SetFloorsMaterial(blueFloorList, Mat_Blue);
        SetFloorsMaterial(greenFloorList, Mat_Green);
    }

    private void SetFloorsMaterial(List<FloorTile> chunk, Material mat)
    {
        foreach (FloorTile floor in chunk)
        {
            floor.SetMat(mat);
        }
    }

    [Button]
    private void RedDropFloorCall()
    {
        StartCoroutine(DropFloor(BossColor.Red, 2));
    }

    [Button]
    private void BlueDropFloorCall()
    {
        StartCoroutine(DropFloor(BossColor.Blue, 2));
    }

    [Button]
    private void GreenDropFloorCall()
    {
        StartCoroutine(DropFloor(BossColor.Green, 2));
    }

    private IEnumerator DropFloor(BossColor type, float delay)
    {
        shuffle = false;
        yield return new WaitForSeconds(delay);

        List<FloorTile> floors = new List<FloorTile>();

        switch (type)
        {
            case BossColor.Red:
                floors = blueFloorList.Concat(greenFloorList).ToList();
                break;
            case BossColor.Blue:
                floors = redFloorList.Concat(greenFloorList).ToList();
                break;
            case BossColor.Green:
                floors = blueFloorList.Concat(redFloorList).ToList();
                break;
        }

        foreach (FloorTile floor in floors)
        {
            yield return new WaitForSeconds(0.005f);
            floor.Drop();
        }

        yield return new WaitForSeconds(7f);
        shuffle = true;
    }
}
