using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Easing { None, EaseIn, EaseOut, EaseInOut }

public class Amplifier_V2 : MonoBehaviour
{
    [Header("Beat Settings")]
    [SerializeField] List<BeatSequence> beatSequence = new List<BeatSequence>();
    [SerializeField] BeatPoint beatPrefab;

    [Header("Parent Reference")]
    [SerializeField] Canvas canvas;
    [SerializeField] Transform sliderVisualParent;

    [Header("Speaker")]
    [SerializeField] private Easing easingMethod;
    [SerializeField] private Minigame_Speaker speaker;
    private Image speakerImg;

    private PlayerController playerController;
    private List<BeatData> beatData = new List<BeatData>();
    private List<BeatPoint> beatObjects = new List<BeatPoint>();
    private int index;
    private bool startGame, isSpawning;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        speakerImg = speaker.GetComponent<Image>();
        isSpawning = true;
    }

    private void OnEnable()
    {
        TempoManager.OnBeat += TempoManager_OnBeat;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LeanTween.reset();
            int rand = Random.Range(0, beatSequence.Count);
            beatData = beatSequence[rand].beatSettings;
            playerController.allowedAction = false;
            startGame = true;
            canvas.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController.allowedAction = true;
            Resetter();
        }
    }

    private void OnDisable()
    {
        TempoManager.OnBeat -= TempoManager_OnBeat;
    }

    private void TempoManager_OnBeat()
    {
        if (!startGame) return;

        if (isSpawning)
        {
            if (index < beatData.Count)
            {
                BeatPoint beatPoint = Instantiate(beatPrefab, canvas.transform);
                beatPoint.rect.anchoredPosition = beatData[index].position;
                speakerImg.transform.SetAsLastSibling();

                if (index < beatData.Count - 1)
                {
                    float timeToBeatCount = TempoManager.GetTimeToBeatCount(beatData[index].beat);
                    PositionUtility.CalculatePositionInfo(beatData[index].position, beatData[index + 1].position, out float xPos, out Direction dir, out float scale);
                    beatPoint.Scale(sliderVisualParent, xPos, dir, scale, timeToBeatCount);
                }

                switch (beatData[index].key)
                {
                    case KeyInput.Circle:
                        beatPoint.img.sprite = SpriteData.Instance.circle;
                        break;
                    case KeyInput.Cross:
                        beatPoint.img.sprite = SpriteData.Instance.cross;
                        break;
                    case KeyInput.Square:
                        beatPoint.img.sprite = SpriteData.Instance.square;
                        break;
                    case KeyInput.Triangle:
                        beatPoint.img.sprite = SpriteData.Instance.triangle;
                        break;
                    case KeyInput.None:
                        beatPoint.img.sprite = SpriteData.Instance.skip;
                        break;
                }

                beatObjects.Add(beatPoint);
                index++;

                if (index > beatData.Count - 1)
                {
                    isSpawning = false;
                    index = -1;
                }
            }
            //else if (index == beatData.Count)
            //{
            //    isSpawning = false;
            //    index = -1;
            //}
        }
        else
        {
            if (index < beatData.Count - 1)
            {
                index++;
                float timeToBeatCount = TempoManager.GetTimeToBeatCount(beatData[index].beat);
                speaker.touchPoint = beatObjects[index].img.rectTransform.position;
                speaker.key = beatData[index].key;

                switch (easingMethod)
                {
                    case Easing.None:
                        speakerImg.rectTransform.LeanMoveLocal(beatData[index].position, timeToBeatCount);
                        break;
                    case Easing.EaseOut:
                        speakerImg.rectTransform.LeanMoveLocal(beatData[index].position, timeToBeatCount).setEaseOutCirc();
                        break;
                    case Easing.EaseInOut:
                        speakerImg.rectTransform.LeanMoveLocal(beatData[index].position, timeToBeatCount).setEaseInOutCirc();
                        break;
                    case Easing.EaseIn:
                        speakerImg.rectTransform.LeanMoveLocal(beatData[index].position, timeToBeatCount).setEaseInCirc();
                        break;
                }
            }
            else if (index == beatData.Count - 1)
            {
                float timeToBeatCount = TempoManager.GetTimeToBeatCount(beatData[index].beat);
                speaker.touchPoint = beatObjects[index].img.rectTransform.position;
                speaker.key = beatData[index].key;

                switch (easingMethod)
                {
                    case Easing.None:
                        speakerImg.rectTransform.LeanMoveLocal(beatData[index].position, timeToBeatCount);
                        break;
                    case Easing.EaseOut:
                        speakerImg.rectTransform.LeanMoveLocal(beatData[index].position, timeToBeatCount).setEaseOutCirc();
                        break;
                    case Easing.EaseInOut:
                        speakerImg.rectTransform.LeanMoveLocal(beatData[index].position, timeToBeatCount).setEaseInOutCirc();
                        break;
                    case Easing.EaseIn:
                        speakerImg.rectTransform.LeanMoveLocal(beatData[index].position, timeToBeatCount).setEaseInCirc();
                        break;
                }

                index++;
            }
        }
    }

    [Button]
    public void Resetter()
    {
        foreach (BeatPoint beat in beatObjects)
        {
            Destroy(beat.gameObject);
        }

        for (int i = 0; i < sliderVisualParent.childCount; i++)
        {
            Destroy(sliderVisualParent.GetChild(i).gameObject);
        }

        speakerImg.rectTransform.anchoredPosition = new Vector2(-900, 0);

        beatObjects.Clear();
        
        index = 0;
        isSpawning = true;
        startGame = false;

        canvas.gameObject.SetActive(false);
    }
}

public enum Direction { Up, Down, Left, Right }

public static class PositionUtility
{
    public static void CalculatePositionInfo(Vector2 startPos, Vector2 endPos, out float centerPositionX, out Direction direction, out float scaleX)
    {
        // Calculate center position
        //centerPosition = (startPos + endPos) / 2f;
        centerPositionX = Vector2.Distance(endPos, startPos) / 2;

        // Determine direction and assign enum Direction
        Vector2 directionVector = endPos - startPos;
        if (Mathf.Abs(directionVector.x) > Mathf.Abs(directionVector.y))
        {
            if (directionVector.x > 0)
            {
                // Right direction
                direction = Direction.Right;
            }
            else
            {
                // Left direction
                direction = Direction.Left;
            }
        }
        else
        {
            if (directionVector.y > 0)
            {
                // Up direction
                direction = Direction.Up;
            }
            else
            {
                // Down direction
                direction = Direction.Down;
            }
        }

        // Calculate scale of x
        scaleX = directionVector.magnitude;
    }
}

