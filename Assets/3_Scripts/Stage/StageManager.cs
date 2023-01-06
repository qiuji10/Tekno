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
    [SerializeField] private Material Mat_default;
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

        #region with specific ratio on specific type
        //int redChunkSize = floorList.Count * 2 / 10;
        //int remainder = floorList.Count % 10;

        //int blueChunkSize = (floorList.Count - redChunkSize) / 2;
        //int greenChunkSize = floorList.Count - redChunkSize - blueChunkSize;

        //// Adjust the sizes of the second and third chunks if necessary
        //if (remainder > 2)
        //{
        //    blueChunkSize++;
        //    greenChunkSize++;
        //}
        //else if (remainder > 0)
        //{
        //    greenChunkSize++;
        //}
        #endregion

        SetFloorsMaterial(redFloorList, Mat_Red);
        SetFloorsMaterial(blueFloorList, Mat_Blue);
        SetFloorsMaterial(greenFloorList, Mat_Green);
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
        SetTileTimer(3, 3);

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
    [Button]
    private void asas()
    {
        //int totalTileLine = Mathf.RoundToInt(Mathf.Sqrt((float)floorList.Count));
        //int firstFloorInLine= 0;

        //for (int i = 1; i <= totalTileLine; i++)
        //{
        //    Debug.Log($"{i}, {firstFloorInLine} / {i * totalTileLine}");
        //    firstFloorInLine = (i * totalTileLine) + 1;
        //}

        StartCoroutine(LineTileDrop(0.1f));
    }

    private IEnumerator LineTileDrop(float delay)
    {
        SetTileTimer(0.7f, 1f);
        SetFloorsMaterial(floorList, Mat_default);
        shuffle = false;
        floorList = floorList.OrderBy(x => x.transform.position.x).ToList();
        yield return new WaitForSeconds(delay);

        int totalTileLine = Mathf.RoundToInt(Mathf.Sqrt((float)floorList.Count));
        int firstFloorInLine = 0;

        for (int i = 1; i <= totalTileLine; i++)
        {
            List<FloorTile> lineFloors = floorList.GetRange(firstFloorInLine, totalTileLine);

            SetFloorsMaterial(lineFloors, Mat_Red);

            firstFloorInLine += totalTileLine;

            yield return new WaitForSeconds(0.35f);

            foreach (FloorTile floor in lineFloors)
            {
                floor.Drop();
            }

            SetFloorsMaterial(lineFloors, Mat_default);
        }

        
    }

    private void SetTileTimer(float downTime, float upTime)
    {
        foreach (FloorTile floor in floorList)
        { 
            floor.dropTimer = downTime;
            floor.liftTimer = upTime;
        }
    }

    private void SetTileTimer(float downTime, float upTime, List<FloorTile> floorList)
    {
        foreach (FloorTile floor in floorList)
        {
            floor.dropTimer = downTime;
            floor.liftTimer = upTime;
        }
    }

    private void SetFloorsMaterial(List<FloorTile> chunk, Material mat)
    {
        foreach (FloorTile floor in chunk)
        {
            floor.SetMat(mat);
        }
    }
}
