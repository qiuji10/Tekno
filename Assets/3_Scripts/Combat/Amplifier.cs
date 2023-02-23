using Cinemachine;
using DG.Tweening;
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

    [Header("Beat Settings")]
    [SerializeField] private List<CircleBeat> beats;
    [SerializeField] private List<BeatSettings> beatSettings;
    [SerializeField] private Canvas beatCanvas;
    [SerializeField] private CircleBeat circleBeatPrefab;

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCameraBase vcam;

    public bool IsAlive => health > 0;

    void OnEnable()
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


        StartCoroutine(Fail(beat));
        
    }

    private void SuccessStreak(CircleBeat beat)
    {
        Damage(25);
    }

    public void Damage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Debug.Log("speaker die4");
            health = 0;

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

    private IEnumerator Fail(CircleBeat beat)
    {
        RectTransform beatRect = beat.GetComponent<RectTransform>();

        float timeToBeat = TempoManager.GetTimeToBeatCount(1);
        float d_timeToBeat = timeToBeat * 2;
        beatRect.DOAnchorPos(Vector2.zero, timeToBeat);
        yield return new WaitForSeconds(d_timeToBeat);
        Vector3 largeScale = new Vector3(4, 4, 0);
        beatRect.DOScale(largeScale, timeToBeat);
        yield return new WaitForSeconds(d_timeToBeat);
        beatRect.DOScale(Vector2.zero, timeToBeat);
        yield return new WaitForSeconds(d_timeToBeat);
    }
}
