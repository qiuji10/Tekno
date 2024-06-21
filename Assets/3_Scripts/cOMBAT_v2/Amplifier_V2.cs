using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.Events;

public enum KeyInput { None, Circle, Cross, Square, Triangle }
public enum Direction { Up, Down, Left, Right }

public class Amplifier_V2 : MonoBehaviour
{
    [Header("Beat Settings")]
    [SerializeField] List<BeatSequence> beatSequence = new List<BeatSequence>();
    [SerializeField] BeatPoint beatPrefab;
    [SerializeField] bool showGizmos;

    [Header("Hijack Succeeded")]
    [SerializeField] private List<EnemyBase> enemiesInControl;
    [SerializeField] private List<BillboardCycle> billboardCycles;
    [SerializeField] private ParticleSystem particle;
    public UnityEvent OnHijackSucceed;

    [Header("Hijack Failed")]
    [SerializeField] private ParticleSystem controlledVFX;
    [SerializeField] private float knockBackRange = 10;
    [SerializeField] private float knockBackPower = 75;

    #region Private Properties
    // UI Reference
    private Canvas canvas;
    private Transform sliderVisualParent;
    private Transform beatVisualParent;
    private TMP_Text countdownText;
    private Image amplifierCoreImg;
    private RectTransform speakerHealthBar;
    private RectTransform amplifierHealthBar;
    private Image speakerImg;

    private Minigame_Speaker speaker;
    private DecalProjector decalProjector;
    private PlayerController playerController;
    private EventInvoker eventInvoker;
    private List<BeatData> beatData = new List<BeatData>();
    private List<BeatPoint> beatObjects = new List<BeatPoint>();
    private List<CustomSlider> speakerSliders = new List<CustomSlider>();
    private List<CustomSlider> amplifierSliders = new List<CustomSlider>();
    private int index, speakerHealth, amplifierHealth, countdown = 4;
    private bool startGame, isSpawning;

    private UI_Shake speakerHealthShake, amplifierHealthShake;
    #endregion

    #region Default Function
    private void Start()
    {
        MinigameData data = MinigameData.Instance;
        canvas = data.canvas;
        sliderVisualParent = data.sliderVisualParent;
        beatVisualParent = data.beatVisualParent;
        countdownText = data.countdownText;
        amplifierCoreImg = data.ampCoreImg;
        speakerHealthBar = data.speakerHealthBar;
        amplifierHealthBar = data.ampHealthBar;
        speaker = data.speaker;

        playerController = FindObjectOfType<PlayerController>();
        eventInvoker = GetComponent<EventInvoker>();
        speakerImg = speaker.GetComponent<Image>();

        speakerHealthShake = speakerHealthBar.GetComponent<UI_Shake>();
        amplifierHealthShake = amplifierHealthBar.GetComponent<UI_Shake>();

        speakerSliders = speakerHealthBar.GetComponentsInChildren<CustomSlider>().ToList();
        amplifierSliders = amplifierHealthBar.GetComponentsInChildren<CustomSlider>().ToList();

        countdownText.gameObject.SetActive(false);

        isSpawning = true;

        speakerHealth = 3;
        amplifierHealth = 3;
        decalProjector = GetComponentInChildren<DecalProjector>();
        decalProjector.material = new Material(decalProjector.material);
        decalProjector.material.SetColor("_Color", Color.red);

        enemiesInControl.Clear();

        Collider[] collideData = Physics.OverlapSphere(transform.position, knockBackRange);

        for (int i = 0; i < collideData.Length; i++)
        {
            if (collideData[i].TryGetComponent(out EnemyBase enemy))
            {
                enemiesInControl.Add(enemy);
            }

            if (collideData[i].TryGetComponent(out BillboardCycle billboardCycle))
            {
                billboardCycles.Add(billboardCycle);
            }
        }
    }

    private void OnEnable()
    {
        TempoManager.OnBeat += TempoManager_OnBeat;
    }

    private void OnDisable()
    {
        TempoManager.OnBeat -= TempoManager_OnBeat;
    } 
    #endregion

    public void StartPlay()
    {
        if (StanceManager.curTrack.genre != Genre.Techno) return;

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
        speakerImg.rectTransform.LeanMoveLocal(new Vector2(-850, 0), timeToBeatCount);
        amplifierCoreImg.rectTransform.LeanMoveLocal(beatData[beatData.Count - 1].position, timeToBeatCount);

        PlayerController.allowedInput = false;
        startGame = true;
        canvas.gameObject.SetActive(true);
    }

    private void Speaker_OnHitFailure()
    {
        speakerHealth--;

        speakerHealthShake.Shake();

        if (Gamepad.current != null) Gamepad.current.SetMotorSpeeds(2f, 3f);

        LeanTween.reset();

        speakerImg.sprite = MinigameData.Instance.speakerOff;
        CustomSlider slider = speakerSliders[speakerHealth];
        float timeToBeatCount = TempoManager.GetTimeToBeatCount(1);
        LeanTween.value(slider.gameObject, 1, 0, timeToBeatCount).setOnUpdate((float blend) => {
            slider.Value = blend;
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

        LeanTween.reset();

        speakerImg.sprite = MinigameData.Instance.speakerSuccess;
        CustomSlider slider = amplifierSliders[amplifierHealth];
        float timeToBeatCount = TempoManager.GetTimeToBeatCount(1);
        LeanTween.value(slider.gameObject, 1, 0, timeToBeatCount).setOnUpdate((float blend) => {
            slider.Value = blend;
        }).setOnComplete(() =>
        {
            InputSystem.PauseHaptics();
            StartCoroutine(EvaluateStatus());
        });
    }

    [Button]
    public void DebugHijack()
    {
        enemiesInControl.Clear();

        Collider[] collideData = Physics.OverlapSphere(transform.position, knockBackRange);

        for (int i = 0; i < collideData.Length; i++)
        {
            if (collideData[i].TryGetComponent(out EnemyBase enemy))
            {
                if (!enemy.IsFree)
                    enemiesInControl.Add(enemy);
            }
        }

        foreach (EnemyBase e in enemiesInControl)
        {
            if (e != null && e.gameObject.activeInHierarchy)
            {
                e.FreeEnemy();
            }
        }

        foreach (BillboardCycle billboard in billboardCycles)
        {
            if (billboard != null && billboard.gameObject.activeInHierarchy)
            {
                billboard.isHijackedSuccessful = true;
            }
        }

    }

    private IEnumerator EvaluateStatus()
    {
        if (amplifierHealth <= 0)
        {
            foreach (CustomSlider speakerSlider in speakerSliders)
            {
                //speakerSlider.value = 1f;
                speakerSlider.Value = 1f;
            }

            foreach (CustomSlider amplifierSlider in amplifierSliders)
            {
                //amplifierSlider.value = 1f;
                amplifierSlider.Value = 1f;
            }

            enemiesInControl.Clear();

            Collider[] collideData = Physics.OverlapSphere(transform.position, knockBackRange);

            for (int i = 0; i < collideData.Length; i++)
            {
                if (collideData[i].TryGetComponent(out EnemyBase enemy))
                {
                    if (!enemy.IsFree)
                        enemiesInControl.Add(enemy);
                }
            }

            foreach (EnemyBase e in enemiesInControl)
            {
                if (e != null && e.gameObject.activeInHierarchy)
                {
                    e.FreeEnemy();
                }
            }

            foreach (BillboardCycle billboard in billboardCycles)
            {
                if (billboard != null && billboard.gameObject.activeInHierarchy)
                {
                    billboard.isHijackedSuccessful = true;
                }
            }

            OnHijackSucceed?.Invoke();
            StartCoroutine(HackedDecal());
            StanceManager.AllowPlayerSwitchStance = true;
            PlayerController.allowedInput = true;
            canvas.gameObject.SetActive(false);
            Resetter();
            amplifierHealth = 3;
            speakerHealth = 3;

            PauseMenu.canPause = true;
            yield return new WaitForSeconds(0.75f);

            enabled = false;

            //eventInvoker.enabled = true;
        }
        else if (speakerHealth <= 0)
        {
            foreach (CustomSlider speakerSlider in speakerSliders)
            {
                speakerSlider.Value = 1f;
            }

            foreach (CustomSlider amplifierSlider in amplifierSliders)
            {
                amplifierSlider.Value = 1f;
            }

            StanceManager.AllowPlayerSwitchStance = true;
            PlayerController.allowedInput = true;
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
                    knockable.Knock(new Vector3(direction.x, 0.5f, direction.z), knockBackPower);
                }
            }

            PauseMenu.canPause = true;
            yield return new WaitForSeconds(0.75f);
            eventInvoker.enabled = true;
        }
        else
        {
            yield return new WaitForSeconds(0.75f);
            Resetter();
            PauseMenu.canPause = false;
            yield return null;
            StartPlay();
        }
    }

    private void TempoManager_OnBeat()
    {
        //controlledVFX.Play();

        if (!startGame) return;

        if (isSpawning)
        {
            if (index < beatData.Count)
            {
                BeatPoint beatPoint = Instantiate(beatPrefab, beatVisualParent.transform);
                beatPoint.rect.anchoredPosition = beatData[index].position;
                
                speakerImg.transform.SetAsLastSibling();
                speakerImg.sprite = MinigameData.Instance.speakerReady;

                MinigameData.Instance.promptText.text = "<color=blue>Hacking in progress...</color>";

                if (index < beatData.Count - 1)
                {
                    float timeToBeatCount = TempoManager.GetTimeToBeatCount(beatData[index].beat);
                    PositionUtility.CalculateSliderInfo(beatData[index].position, beatData[index + 1].position, out float xPos, out Direction dir, out float scale);
                    beatPoint.Scale(1, sliderVisualParent, xPos, dir, scale, timeToBeatCount);
                }

                switch (beatData[index].key)
                {
                    case KeyInput.Circle:   beatPoint.img.sprite = SpriteData.Instance.circle;      beatPoint.img.color = HexColor("#DE3839");    break;
                    case KeyInput.Cross:    beatPoint.img.sprite = SpriteData.Instance.cross;       beatPoint.img.color = HexColor("#7EABE1");    break;
                    case KeyInput.Square:   beatPoint.img.sprite = SpriteData.Instance.square;      beatPoint.img.color = HexColor("#CE8ED6");    break;
                    case KeyInput.Triangle: beatPoint.img.sprite = SpriteData.Instance.triangle;    beatPoint.img.color = HexColor("#4ADB7B");    break;
                    case KeyInput.None:     beatPoint.img.sprite = SpriteData.Instance.skip;        beatPoint.img.color = Color.white;             break;
                }

                beatObjects.Add(beatPoint);
                index++;

                if (index == beatData.Count)
                {
                    countdownText.text = "GO!";
                }
                else if (index > beatData.Count - countdown)
                {
                    if (!countdownText.gameObject.activeInHierarchy)
                    {
                        countdownText.gameObject.SetActive(true);
                    }
                    countdown--;
                    countdownText.text = countdown.ToString();
                }

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
                if (countdownText.gameObject.activeInHierarchy)
                {
                    countdownText.gameObject.SetActive(false);
                }

                speaker.lastHitTime = Time.time;
            }

            if (index < beatData.Count - 1)
            {
                speaker.startTrace = true;
                speaker.currentBeat++;
                index++;
                float timeToBeatCount = TempoManager.GetTimeToBeatCount(beatData[index].beat);
                timeToBeatCount = 0.1f;
                //float timeToBeatCount = (60f / 140f);
                //speaker.touchPoint = beatObjects[index].img.rectTransform.position;
                //speaker.key = beatData[index].key;
                //speaker.beatPoint = beatObjects[index];

                speaker.beatPoint = beatObjects[index];

                if (index > 0)
                {
                    speaker.prevTouchPoint = beatObjects[index - 1].img.rectTransform.position;
                }

                speaker.touchPoint = beatObjects[index].img.rectTransform.position;
                speaker.key = beatData[index].key;

                StartCoroutine(delayBeatSwitch());

                int indexCount = index;

                speakerImg.sprite = MinigameData.Instance.speakerOn;

                if (index > 0)
                {
                    if (index < beatData.Count - 1)
                        indexCount = index - 1;

                    if (index == beatData.Count - 1)
                    {
                        indexCount--;
                    }

                    PositionUtility.CalculateSliderInfo(beatData[indexCount].position, beatData[indexCount + 1].position, out float xPos, out Direction dir, out float scale);
                    beatObjects[indexCount].Scale(2, sliderVisualParent, xPos, dir, scale, timeToBeatCount);
                }
                
                //speakerImg.rectTransform.LeanMoveLocal(beatData[index].position, timeToBeatCount).setEaseOutCirc();
                speaker.speakerMovement = speakerImg.rectTransform.LeanMoveLocal(beatData[index].position, timeToBeatCount).setEaseOutExpo();

                //StartCoroutine(delayMoveSpeaker());

            }
            else if (index == beatData.Count - 1)
            {
                speaker.currentBeat++;
                index++;
            }
        }
    }

    [Button]
    public void Resetter()
    {
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

        countdown = 4;
        index = 0;
        isSpawning = true;
        startGame = false;

        speaker.startTrace = false;
        speaker.failed = false;

        //canvas.gameObject.SetActive(false);
    }

    private IEnumerator delayMoveSpeaker()
    {
        float timeToBeatCount = (60f / 140f) * 0.5f;

        yield return new WaitForSeconds(timeToBeatCount);

        speakerImg.rectTransform.LeanMoveLocal(beatData[index].position, timeToBeatCount);

        speaker.startTrace = true;
    }

    private IEnumerator delayBeatSwitch()
    {
        yield return new WaitForSeconds(0.2f);

        speaker.key = beatData[index].key;

    }

    #region Utility
    private IEnumerator HackedDecal()
    {
        float timer = 0;
        float fadeTime = 1;
        string radius = "_Radius";

        while (decalProjector.material.GetFloat(radius) > 0)
        {
            timer -= Time.deltaTime;
            float ratio = Mathf.Clamp(timer / fadeTime, 0f, 1f);
            decalProjector.material.SetFloat(radius, ratio);
            yield return null;
        }

        decalProjector.material.SetFloat(radius, 0);
        decalProjector.material.SetColor("_Color", Color.blue);
        timer = 0;

        decalProjector.material.SetTexture("_Base_Map", MinigameData.Instance.speakerDecal);

        while (decalProjector.material.GetFloat(radius) < 1)
        {
            timer += Time.deltaTime;
            float ratio = Mathf.Clamp(timer / fadeTime, 0f, 1f);
            decalProjector.material.SetFloat(radius, ratio);
            yield return null;
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

    private static Color HexColor(string code)
    {
        ColorUtility.TryParseHtmlString(code, out Color color);
        return color;
    }

    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, knockBackRange);
        }
    }
    #endregion
}

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