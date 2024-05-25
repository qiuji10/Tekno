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
    [Header("Settings")]
    [SerializeField] List<BeatSequence> beatSequence = new List<BeatSequence>();
    [SerializeField] BeatPoint beatPrefab;
    [SerializeField] bool isPlaying;
    [SerializeField] bool showGizmos;

    [Header("Hijack Succeeded")]
    [SerializeField] private List<EnemyBase> enemiesInControl;
    [SerializeField] private List<BillboardCycle> billboardCycles;
    [SerializeField] private ParticleSystem particle;
    public UnityEvent OnHijackSucceed;

    [Header("Hijack Failed")]
    [SerializeField] private ParticleSystem controlledVFX;
    [SerializeField] private float knockBackRange = 10;

    [Header("Misc")]
    [SerializeField] private Texture2D ampTex;
    [SerializeField] private Texture2D hijackedTex;

    #region Private Properties
    // UI Reference
    private DecalProjector decalProjector;
    private EventInvoker eventInvoker;
    #endregion

    #region Default Function
    private void Awake()
    {
        eventInvoker = GetComponent<EventInvoker>();
        decalProjector = GetComponentInChildren<DecalProjector>();

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
        Minigame.OnMinigameStart += OnGameStart;
        Minigame.OnMinigameEnd += OnGameEnd;
    }

    private void OnDisable()
    {
        Minigame.OnMinigameStart -= OnGameStart;
        Minigame.OnMinigameEnd -= OnGameEnd;
    }

    #endregion

    public void StartPlay()
    {
        if (StanceManager.curTrack.genre != Genre.Techno) return;

        isPlaying = true;

        Minigame.StartGame(beatSequence);
    }

    private void OnGameStart()
    {
        if (isPlaying)
        {
            PauseMenu.canPause = false;
            StanceManager.AllowPlayerSwitchStance = false;
            PlayerController.allowedInput = false;
            eventInvoker.enabled = false;
        }
    }

    private void OnGameEnd(Minigame.State state)
    {
        if (isPlaying)
        {
            PauseMenu.canPause = true;
            StanceManager.AllowPlayerSwitchStance = true;
            PlayerController.allowedInput = true;

            if (state == Minigame.State.Success)
            {
                OnHijackSucceed?.Invoke();
                StartCoroutine(HackedDecal());
                FreeEnemies();
            }
            else if (state == Minigame.State.Fail)
            {
                eventInvoker.enabled = true;
            }

            isPlaying = false;
        }
    }

    [Button]
    public void DebugHijack()
    {
        FreeEnemies();
        HijackBillboard();
    }

    public void FreeEnemies()
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
    }

    public void HijackBillboard()
    {
        foreach (BillboardCycle billboard in billboardCycles)
        {
            if (billboard != null && billboard.gameObject.activeInHierarchy)
            {
                billboard.isHijackedSuccessful = true;
            }
        }
    }

    private void InitDecal()
    {
        decalProjector.material.SetColor("_Color", Color.red);
        decalProjector.material.SetFloat("_Radius", 1);
        decalProjector.material.SetTexture("_Base_Map", ampTex);
    }

    #region Utility
    private IEnumerator HackedDecal()
    {
        float timer = 0;
        float fadeTime = 1;
        string radius = "_Radius";

        decalProjector.material = new Material(decalProjector.material);

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

        decalProjector.material.SetTexture("_Base_Map", hijackedTex);

        while (decalProjector.material.GetFloat(radius) < 1)
        {
            timer += Time.deltaTime;
            float ratio = Mathf.Clamp(timer / fadeTime, 0f, 1f);
            decalProjector.material.SetFloat(radius, ratio);
            yield return null;
        }
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