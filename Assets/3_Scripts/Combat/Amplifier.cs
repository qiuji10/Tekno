using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeyInput { Circle, Cross, Square, Triangle }

[Serializable]
public class BeatSettings
{
    public KeyInput key;
    public int onBeatCount;
    public Vector2 position;
}

public class Amplifier : MonoBehaviour, IDamagable
{
    [Header("Combat")]
    [SerializeField] private int health = 100;
    [SerializeField] private List<EnemyBase> enemiesInControl;
    [SerializeField] private float knockBackRange = 10;
    [SerializeField] private float knockBackPower = 75;

    [Header("Beat Settings")]
    [SerializeField] private List<CircleBeat> beats;
    [SerializeField] private List<BeatSettings> beatSettings;
    [SerializeField] private Canvas beatCanvas;
    [SerializeField] private CircleBeat circleBeatPrefab;

    [Header("Visuals")]
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private CinemachineVirtualCameraBase vcam;

    private PlayerController_FixedCam player;

    public bool IsAlive => health > 0;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController_FixedCam>();
    }

    void OnEnable()
    {
        LeanTween.reset();
        ResetAmplifier();
    }

    void OnDisable()
    {
        for (int i = 0; i < beats.Count; i++)
        {
            beats[i].successCallback -= SuccessStreak;
            beats[i].failCallback -= FailStreak;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ResetAmplifier();
            vcam.Priority = 11;
            player.allowedAction = false;
            beatCanvas.gameObject.SetActive(true);
            beats[0].startTrace = true;
        }
    }

    private void FailStreak(CircleBeat beat)
    {
        for (int i = 0; i < beats.Count; i++)
        {
            beats[i].end = true;
        }

        vcam.Priority = 9;
        player.allowedAction = true;

        health = 100;
        StartCoroutine(Fail(beat));
        
    }

    private void SuccessStreak(CircleBeat beat)
    {
        //beat.rect.DOScale(Vector2.zero, TempoManager.GetTimeToBeatCount(1));
        LeanTween.scale(beat.rect, Vector2.zero, TempoManager.GetTimeToBeatCount(1));
        Damage(25);
    }

    public void Damage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Debug.Log("speaker die4");
            vcam.Priority = 9;
            player.allowedAction = true;

            // TODO: Temporary make heaoth 100 and keep canvas true to loop, in future the amplifier should 1 time destroyed
            health = 100;
            //beatCanvas.gameObject.SetActive(false);
            

            if (enemiesInControl.Count > 0)
            {
                foreach (EnemyBase enemy in enemiesInControl)
                {
                    enemy.FreeEnemy();
                }
            }
            else
            {
                Debug.Log("no enemies");
            }
        }
    }

    private void ResetAmplifier()
    {
        beats.Clear();

        if (beatCanvas.transform.childCount > 0)
        {
            for (int i = 0; i < beatCanvas.transform.childCount; i++)
            {
                Destroy(beatCanvas.transform.GetChild(i).gameObject);
            }
        }

        for (int i = 0; i < beatSettings.Count; i++)
        {
            CircleBeat beat = Instantiate(circleBeatPrefab, beatCanvas.transform);

            if (i != 0)
                beat.gameObject.SetActive(false);

            beats.Add(beat);
        }

        for (int i = 0; i < beats.Count; i++)
        {
            beats[i].key = beatSettings[i].key;
            beats[i].onBeatCount = beatSettings[i].onBeatCount;
            beats[i].GetComponent<RectTransform>().anchoredPosition = beatSettings[i].position;
            beats[i].failCallback += FailStreak;
            beats[i].successCallback += SuccessStreak;

            if (i == beats.Count - 1)
            {
                break;
            }

            beats[i].nextBeat = beats[i + 1];
        }
    }

    private IEnumerator Fail(CircleBeat beat)
    {
        RectTransform beatRect = beat.GetComponent<RectTransform>();

        float timeToBeat = TempoManager.GetTimeToBeatCount(1);
        float d_timeToBeat = timeToBeat * 2;

        //Sequence sequence = DOTween.Sequence();
        //foreach (CircleBeat b in beats)
        //{
        //    if (b.Equals(beat)) continue;

        //    sequence.Join(b.rect.DOScale(Vector2.zero, timeToBeat));
        //}

        //sequence.Join(beatRect.DOAnchorPos(Vector2.zero, timeToBeat));
        //sequence.SetUpdate(UpdateType.Fixed);

        foreach (CircleBeat b in beats)
        {
            if (b.Equals(beat)) continue;

            LeanTween.scale(b.rect, Vector2.zero, timeToBeat);
        }

        LeanTween.move(beatRect, Vector2.zero, timeToBeat);

        yield return new WaitForSeconds(d_timeToBeat);
        
        Vector3 largeScale = new Vector3(4, 4, 0);
        //beatRect.DOScale(largeScale, timeToBeat);
        LeanTween.scale(beatRect, largeScale, timeToBeat);
        
        yield return new WaitForSeconds(d_timeToBeat);
        //beatRect.DOScale(Vector2.zero, timeToBeat);
        LeanTween.scale(beatRect, Vector2.zero, timeToBeat);
        yield return new WaitForSeconds(d_timeToBeat);

        beatCanvas.gameObject.SetActive(false);
        particle.Play();

        Collider[] collideData = Physics.OverlapSphere(transform.position, knockBackRange);

        for (int i = 0; i < collideData.Length; i++)
        {
            if (collideData[i].TryGetComponent(out IKnockable knockable))
            {
                Vector3 direction = (collideData[i].transform.position - transform.position).normalized;
                knockable.Knock(direction, knockBackPower);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, knockBackRange);
    }
}
