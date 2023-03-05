using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Amplifier_V2 : MonoBehaviour
{
    [SerializeField] List<BeatData> beatSequence = new List<BeatData>();
    [SerializeField] BeatPoint beatPrefab;
    
    [SerializeField] Canvas canvas;
    [SerializeField] Transform sliderVisualParent;

    [SerializeField] private Minigame_Speaker speaker;
    private Image speakerImg;

    private PlayerController playerController;
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
            if (index < beatSequence.Count)
            {
                BeatPoint beatPoint = Instantiate(beatPrefab, canvas.transform);
                beatPoint.rect.anchoredPosition = beatSequence[index].position;
                speakerImg.transform.SetAsLastSibling();

                if (index < beatSequence.Count - 1)
                {
                    float timeToBeatCount = TempoManager.GetTimeToBeatCount(beatSequence[index].beat);
                    PositionUtility.CalculatePositionInfo(beatSequence[index].position, beatSequence[index + 1].position, out float xPos, out Direction dir, out float scale);
                    beatPoint.Scale(sliderVisualParent, xPos, dir, scale, timeToBeatCount);
                }

                switch (beatSequence[index].key)
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
                        beatPoint.img.sprite = SpriteData.Instance.triangle;
                        beatPoint.img.color = new Color(1, 1, 1, 0);
                        break;
                }

                beatObjects.Add(beatPoint);
                index++;
            }
            else if (index == beatSequence.Count)
            {
                isSpawning = false;
                index = -1;
            }
        }
        else
        {
            if (index < beatSequence.Count - 1)
            {
                index++;
                float timeToBeatCount = TempoManager.GetTimeToBeatCount(beatSequence[index].beat);
                speaker.touchPoint = beatObjects[index].img.rectTransform.position;
                speaker.key = beatSequence[index].key;
                speakerImg.rectTransform.LeanMoveLocal(beatSequence[index].position, timeToBeatCount).setEaseInCirc();
            }
            else if (index == beatSequence.Count - 1)
            {
                float timeToBeatCount = TempoManager.GetTimeToBeatCount(beatSequence[index].beat);
                speaker.touchPoint = beatObjects[index].img.rectTransform.position;
                speaker.key = beatSequence[index].key;
                speakerImg.rectTransform.LeanMoveLocal(beatSequence[index].position, timeToBeatCount).setEaseInCirc();
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

        canvas.gameObject.SetActive(true);
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

