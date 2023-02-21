using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amplifier : MonoBehaviour, IDamagable
{
    [SerializeField] private int health = 100;
    [SerializeField] private List<EnemyBase> enemiesInControl;

    public bool IsAlive => health > 0;
    public float bufferMargin = 0.3f;

    private bool _hasDetectedInput = false;
    private float _lastBeatTime = 0f;

    private TempoManager tempoManager;

    private void Awake()
    {
        tempoManager = FindObjectOfType<TempoManager>();
    }

    void OnEnable()
    {
        TempoManager.OnBeat += OnBeat;
    }

    void OnDisable()
    {
        TempoManager.OnBeat -= OnBeat;
    }

    private void OnBeat()
    {
        _lastBeatTime = Time.time;
        _hasDetectedInput = false;
    }

    private void Update()
    {
        
    }

    public void Damage(int damage)
    {
        if (!_hasDetectedInput)
        {
            float timeSinceLastBeat = Time.time - _lastBeatTime;
            float margin = TempoManager.BeatsPerMinuteToDelay(tempoManager.BPM) * bufferMargin;
            //Debug.Log(margin);
            if (timeSinceLastBeat > margin * 2f)
            {
                Debug.Log($"timeSinceLastBeat:{timeSinceLastBeat}, {margin} <color=red>Input too late</color>");
            }
            else if (timeSinceLastBeat >= margin)
            {
                Debug.Log($"timeSinceLastBeat:{timeSinceLastBeat}, {margin} <color=green>Input on beat</color>");
            }
            else if (timeSinceLastBeat < margin && timeSinceLastBeat > margin / 2f)
            {
                Debug.Log($"timeSinceLastBeat:{timeSinceLastBeat}, {margin} <color=green>Input a bit early</color>");
            }
            else
            {
                Debug.Log($"timeSinceLastBeat:{timeSinceLastBeat}, {margin} <color=yellow>Input too early</color>");
            }

            _hasDetectedInput = true;
        }

        if (health > 0)
        {
            health -= damage;
        }
        else
        {
            health = 0;

            if (enemiesInControl != null)
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
}
