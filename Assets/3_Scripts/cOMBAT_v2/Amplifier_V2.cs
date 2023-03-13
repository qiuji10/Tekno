using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.UI;

public enum KeyInput { None, Circle, Cross, Square, Triangle }

public class Amplifier_V2 : MonoBehaviour
{
    [Header("Beat Settings")]
    [SerializeField] List<BeatSequence> beatSequence = new List<BeatSequence>();
    [SerializeField] BeatPoint beatPrefab;

    [Header("UI Reference")]
    [SerializeField] Canvas canvas;
    [SerializeField] Transform sliderVisualParent;
    [SerializeField] Transform beatVisualParent;
    [SerializeField] Image amplifierCoreImg;
    [SerializeField] RectTransform speakerHealthBar;
    [SerializeField] RectTransform amplifierHealthBar;
    
    [Header("Speaker")]
    [SerializeField] private Minigame_Speaker speaker;
    private Image speakerImg;

    [Header("Hijack Successed")]
    [SerializeField] private List<EnemyBase> enemiesInControl;
    [SerializeField] private ParticleSystem particle;

    [Header("Hijack Failed")]
    [SerializeField] private float knockBackRange = 10;
    [SerializeField] private float knockBackPower = 75;

    private PlayerController playerController;
    private EventInvoker eventInvoker;
    private List<BeatData> beatData = new List<BeatData>();
    private List<BeatPoint> beatObjects = new List<BeatPoint>();
    private List<Slider> speakerSliders = new List<Slider>();
    private List<Slider> amplifierSliders = new List<Slider>();
    private int index, totalRound, speakerHealth, amplifierHealth;
    private bool startGame, isSpawning;

    private UI_Shake speakerHealthShake, amplifierHealthShake;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        eventInvoker = GetComponent<EventInvoker>();
        speakerImg = speaker.GetComponent<Image>();

        speakerHealthShake = speakerHealthBar.GetComponent<UI_Shake>();
        amplifierHealthShake = amplifierHealthBar.GetComponent<UI_Shake>();

        speakerSliders = speakerHealthBar.GetComponentsInChildren<Slider>().ToList();
        amplifierSliders = amplifierHealthBar.GetComponentsInChildren<Slider>().ToList();

        isSpawning = true;

        speakerHealth = 3;
        amplifierHealth = 3;
        totalRound = speakerHealth + amplifierHealth;
    }

    private void OnEnable()
    {
        TempoManager.OnBeat += TempoManager_OnBeat;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Resetter();
        }
    }

    private void OnDisable()
    {
        TempoManager.OnBeat -= TempoManager_OnBeat;
    }

    public void StartPlay()
    {
        LeanTween.reset();
        StanceManager.AllowPlayerSwitchStance = false;

        eventInvoker.enabled = false;

        //int rand = Random.Range(0, beatSequence.Count);
        int rand = 3 - amplifierHealth;
        beatData = beatSequence[rand].beatSettings;

        int totalInputNeeded = 0;

        foreach (BeatData beat in beatData)
        {
            if (beat.key != KeyInput.None)
                totalInputNeeded++;
        }

        speaker.OnHitFailure += Speaker_OnHitFailure;
        speaker.OnComboSuccess += Speaker_OnComboSuccess;
        speaker.totalInputNeeded = totalInputNeeded;
        speaker.successInput = 0;
        speaker.totalBeat = beatData.Count;
        speaker.currentBeat = 0;
        speaker.touchPoint = beatData[index].position;

        float timeToBeatCount = TempoManager.GetTimeToBeatCount(1);
        speakerImg.rectTransform.LeanMoveLocal(new Vector2(-900, 0), timeToBeatCount);
        amplifierCoreImg.rectTransform.LeanMoveLocal(beatData[beatData.Count - 1].position, timeToBeatCount);

        playerController.allowedInput = false;
        startGame = true;
        canvas.gameObject.SetActive(true);
    }

    private void Speaker_OnHitFailure()
    {
        speakerHealth--;

        speakerHealthShake.Shake();

        if (Gamepad.current != null) Gamepad.current.SetMotorSpeeds(2f, 3f);

        Slider slider = speakerHealthBar.transform.GetChild(speakerHealth).GetComponent<Slider>();
        float timeToBeatCount = TempoManager.GetTimeToBeatCount(1);
        LeanTween.value(slider.gameObject, 1, 0, timeToBeatCount).setOnUpdate((float blend) => {
            slider.value = blend;
        }).setOnComplete(() =>
        {
            InputSystem.PauseHaptics();
            StartCoroutine(EvaluateStatus());
        });
    }

    // This function did not subscribe any event, it is internally called
    private void Speaker_OnComboSuccess()
    {
        amplifierHealth--;

        amplifierHealthShake.Shake();

        if (Gamepad.current != null) Gamepad.current.SetMotorSpeeds(2f, 3f);

        Slider slider = amplifierHealthBar.transform.GetChild(amplifierHealth).GetComponent<Slider>();
        float timeToBeatCount = TempoManager.GetTimeToBeatCount(1);
        LeanTween.value(slider.gameObject, 1, 0, timeToBeatCount).setOnUpdate((float blend) => {
            slider.value = blend;
        }).setOnComplete(() =>
        {
            InputSystem.PauseHaptics();
            StartCoroutine(EvaluateStatus());
        });
    }

    private IEnumerator EvaluateStatus()
    {
        if (amplifierHealth <= 0)
        {
            foreach (Slider speakerSlider in speakerSliders)
            {
                speakerSlider.value = 1f;
            }

            foreach (Slider amplifierSlider in amplifierSliders)
            {
                amplifierSlider.value = 1f;
            }

            foreach (EnemyBase e in enemiesInControl)
            {
                if (e != null)
                {
                    e.FreeEnemy();
                }
            }

            StanceManager.AllowPlayerSwitchStance = true;
            canvas.gameObject.SetActive(false);
            Resetter();
            amplifierHealth = 3;
            speakerHealth = 3;
        }
        else if (speakerHealth <= 0)
        {
            foreach (Slider speakerSlider in speakerSliders)
            {
                speakerSlider.value = 1f;
            }

            foreach (Slider amplifierSlider in amplifierSliders)
            {
                amplifierSlider.value = 1f;
            }

            StanceManager.AllowPlayerSwitchStance = true;
            canvas.gameObject.SetActive(false);
            Resetter();
            amplifierHealth = 3;
            speakerHealth = 3;

            particle.Play();

            Collider[] collideData = Physics.OverlapSphere(transform.position, knockBackRange);

            for (int i = 0; i < collideData.Length; i++)
            {
                if (collideData[i].TryGetComponent(out IKnockable knockable))
                {
                    Vector3 direction = (collideData[i].transform.position - transform.position).normalized;
                    knockable.Knock(new Vector3(direction.x, 0.1f, direction.z), knockBackPower);
                }
            }
        }
        else
        {
            yield return new WaitForSeconds(0.75f);
            Resetter();
            yield return null;
            StartPlay();
        }
    }

    private void TempoManager_OnBeat()
    {
        if (!startGame) return;

        if (isSpawning)
        {
            if (index < beatData.Count)
            {
                BeatPoint beatPoint = Instantiate(beatPrefab, beatVisualParent.transform);
                beatPoint.rect.anchoredPosition = beatData[index].position;
                speakerImg.transform.SetAsLastSibling();

                if (index < beatData.Count - 1)
                {
                    float timeToBeatCount = TempoManager.GetTimeToBeatCount(beatData[index].beat);
                    PositionUtility.CalculateSliderInfo(beatData[index].position, beatData[index + 1].position, out float xPos, out Direction dir, out float scale);
                    beatPoint.Scale(sliderVisualParent, xPos, dir, scale, timeToBeatCount);
                }

                switch (beatData[index].key)
                {
                    case KeyInput.Circle:   beatPoint.img.sprite = SpriteData.Instance.circle;  break;
                    case KeyInput.Cross:    beatPoint.img.sprite = SpriteData.Instance.cross;   break;
                    case KeyInput.Square:   beatPoint.img.sprite = SpriteData.Instance.square;  break;
                    case KeyInput.Triangle: beatPoint.img.sprite = SpriteData.Instance.triangle;break;
                    case KeyInput.None:     beatPoint.img.sprite = SpriteData.Instance.skip;    break;
                }

                beatObjects.Add(beatPoint);
                index++;

                if (index > beatData.Count - 1)
                {
                    isSpawning = false;
                    index = -1;
                }
            }
        }
        else
        {
            if (speaker.failed)
            {
                return;
            }

            if (index == -1)
            {
                speaker.lastHitTime = Time.time;
            }

            if (index < beatData.Count - 1)
            {
                speaker.startTrace = true;
                speaker.currentBeat++;
                index++;
                float timeToBeatCount = TempoManager.GetTimeToBeatCount(beatData[index].beat);
                speaker.touchPoint = beatObjects[index].img.rectTransform.position;
                speaker.key = beatData[index].key;
                
                //if (beatData[index].key == KeyInput.None)
                //{
                //    float timeToInput = GetTimeToInput(index);
                //    Debug.Log(timeToInput);
                //    speaker.timeToInput = timeToInput;
                //}

                speakerImg.rectTransform.LeanMoveLocal(beatData[index].position, timeToBeatCount).setEaseOutCirc();

            }
            else if (index == beatData.Count - 1)
            {
                //Debug.Log("<color=cyan>stop trace</color>");
                //speaker.startTrace = false;
                speaker.currentBeat++;
                index++;
            }
        }
    }

    private float GetTimeToInput(int index)
    {
        float timeToBeatCount = TempoManager.GetTimeToBeatCount(beatData[index].beat);

        if (beatData[index].key == KeyInput.None)
        {
            return timeToBeatCount + GetTimeToInput(index + 1);
        }
        else
        {
            return timeToBeatCount;
        }
    }

    [Button]
    public void Resetter()
    {
        playerController.allowedInput = true;
        eventInvoker.enabled = true;
        speaker.OnHitFailure -= Speaker_OnHitFailure;
        speaker.OnComboSuccess -= Speaker_OnComboSuccess;

        foreach (BeatPoint beat in beatObjects)
        {
            Destroy(beat.gameObject);
        }

        for (int i = 0; i < sliderVisualParent.childCount; i++)
        {
            Destroy(sliderVisualParent.GetChild(i).gameObject);
        }

        beatObjects.Clear();
        
        index = 0;
        isSpawning = true;
        startGame = false;

        speaker.startTrace = false;
        speaker.failed = false;

        canvas.gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, knockBackRange);
    }
}

public enum Direction { Up, Down, Left, Right }

public static class PositionUtility
{
    public static void CalculateSliderInfo(Vector2 startPos, Vector2 endPos, out float centerPositionX, out Direction direction, out float scaleX)
    {
        // Calculate center position
        //centerPosition = (startPos + endPos) / 2f;
        centerPositionX = Vector2.Distance(endPos, startPos) / 2;

        Vector2 directionVector = endPos - startPos;
        if (Mathf.Abs(directionVector.x) > Mathf.Abs(directionVector.y))
        {
            if (directionVector.x > 0)
            {
                direction = Direction.Right;
            }
            else
            {
                direction = Direction.Left;
            }
        }
        else
        {
            if (directionVector.y > 0)
            {
                direction = Direction.Up;
            }
            else
            {
                direction = Direction.Down;
            }
        }

        scaleX = directionVector.magnitude;
    }
}

