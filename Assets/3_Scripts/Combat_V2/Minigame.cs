using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using System;

public partial class Minigame : MonoBehaviour
{
    public enum State { None, Spawn, Play, Fail, Success }

    [SerializeField, ReadOnly] State state = State.None;
    [SerializeField, ReadOnly] private Input input;
    [SerializeField, ReadOnly] int speakerHealth;
    [SerializeField, ReadOnly] int amplifierHealth;

    [Header("Data")]
    [SerializeField] List<BeatSequence> data;
    [SerializeField] List<BeatData> beatDatas;

    [Header("Managers")]
    [SerializeField] Minigame_Visuals visual;
    [SerializeField] Minigame_Mover mover;

    [Header("Input")]
    [SerializeField] InputReference inputReference;

    [Header("Speakers")]
    [SerializeField] RectTransform speaker;
    [SerializeField] RectTransform speakerOriPos;

    [Header("Amplifier")]
    [SerializeField] RectTransform amp;

    private int level;
    private int beatCount;

    private List<GameObject> beatNotes = new();
    private List<BeatPath> beatPaths = new();

    //public float beatTime => TempoManager.GetTimeToBeatCount(1) * 2.33f;
    public float beatTime => 60f / 140f;

    public static event Action OnMinigameStart;
    public static event Action<State> OnMinigameEnd;

    private static event Action<List<BeatSequence>> OnGameEventStart;

    private void Awake()
    {
        visual.OpenPanel(false);
    }

    private void OnEnable()
    {
        TempoManager.OnBeat += TempoManager_OnBeat;
        OnGameEventStart += StartMiniGame;
    }

    private void OnDisable()
    {
        TempoManager.OnBeat -= TempoManager_OnBeat;
        OnGameEventStart -= StartMiniGame;
    }

    private void Update()
    {
        if (input != null)
            input.Process(beatCount);
    }

    public static void StartGame(List<BeatSequence> data) => OnGameEventStart?.Invoke(data);

    public void StartMiniGame(List<BeatSequence> data)
    {
        this.data = data;

        OnMinigameStart?.Invoke();
        visual.OpenPanel(true);

        beatCount = 0;
        level = 0;
        speakerHealth = 3;
        amplifierHealth = 3;

        visual.SetSpeakerHP(speakerHealth);
        visual.SetAmplifierHP(speakerHealth);

        CheckLevel();
    }

    private void CheckLevel()
    {
        if (level < data.Count)
        {
            beatDatas = data[level].beatSettings;
            level++;

            ResetMiniGame();
            SpawnNotes(level);

            state = State.Spawn;
        }
        else
        {
            // End
            if (amplifierHealth <= 0)
            {
                visual.OpenPanel(false);
                state = State.Success;
                OnMinigameEnd?.Invoke(state);
            }
        }
    }

    private void InitSetup()
    {
        ResetMiniGame();

        if (speakerHealth > 0 && amplifierHealth > 0)
        {
            SpawnNotes(level);
            state = State.Spawn;
        }
        else
        {
            level = 0;
            state = State.None;
        }
    }

    private void OnComboSuccess()
    {
        amplifierHealth--;

        visual.SetSpeakerImg(SpeakerStatus.Success);

        VibrateManager.instance.Rumble(5, 10, beatTime);
        visual.ShakeAmplifierHP();
        visual.SetAmplifierHP(amplifierHealth, null);
    }

    private void OnSuccess()
    {
        visual.SetSpeakerImg(SpeakerStatus.On);
        visual.PlaySucessVFX();
    }

    private void OnFail(string msg)
    {
        speakerHealth--;

        VibrateManager.instance.Rumble(5, 10, beatTime);
        visual.SetSpeakerImg(SpeakerStatus.Off);
        visual.PlayFailVFX();
        visual.SetPromptText(msg);
        visual.ShakeSpeakerHP();
        visual.SetSpeakerHP(speakerHealth, null);
        mover.Cancel(speaker);

        state = State.Fail;

        int index = 0;

        if (beatCount > 0)
            index = beatCount - 1;

        beatPaths[Mathf.Clamp(index, 0, beatPaths.Count - 1)].Cancel();
        
        if (speakerHealth > 0)
        {
            beatCount = 0;
        }
        else
        {
            // Lose
            Invoke(nameof(SetFail), beatTime * 3);
        }
    }

    void SetFail()
    {
        visual.OpenPanel(false);
        state = State.Fail;
        OnMinigameEnd?.Invoke(state);
    }

    private void TempoManager_OnBeat()
    {
        if (state == State.Spawn)
        {
            beatNotes[beatCount].SetActive(true);

            if (beatCount < beatPaths.Count)
                beatPaths[beatCount].SetPreviewValue(0, 1, beatTime);

            beatCount++;

            if (beatCount < beatDatas.Count)
                visual.LerpArrow(beatDatas[beatCount - 1].position, beatDatas[beatCount].position, beatTime);

            Countdown();

            if (beatCount > beatNotes.Count - 1)
            {
                beatCount = -1;
                state = State.Play;
            }
        }
        else if (state == State.Play)
        {
            if (beatCount <= -1)
            {
                input = new Input(inputReference);
                input.Init(beatDatas, speaker);
                input.OnComboSuccess += OnComboSuccess;
                input.OnBeatSuccess += OnSuccess;
                input.OnBeatFailure += OnFail;
                input.startTrace = true;
                visual.EnableCountdown(false);
            }

            if (beatCount < beatNotes.Count && beatCount >= 0)
                beatNotes[beatCount].gameObject.SetActive(true);

            if (beatCount < beatPaths.Count && beatCount >= 0)
                beatPaths[beatCount].SetPathValue(0, 1, beatTime);

            beatCount++;

            if (beatCount < beatDatas.Count)
            {
                mover.MoveLocal(speaker, beatDatas[beatCount].position, beatTime);
            }
            else if (beatCount > beatDatas.Count)
            {
                CheckLevel();
            }
        }
        else if (state == State.Fail)
        {
            beatCount++;

            if (beatCount >= 3)
            {
                visual.SetPromptText("");
                InitSetup();
            }
        }
    }

    #region Init Map
    private void SpawnNotes(int index)
    {
        for (int i = 0; i < beatDatas.Count; i++)
        {
            GameObject beatNote = visual.GetBeatNote(beatDatas[i]);
            beatNotes.Add(beatNote);

            if (i < beatDatas.Count - 1)
            {
                BeatPath beatPath = visual.GetBeatPath(beatDatas[i].position, beatDatas[i + 1].position);
                beatPaths.Add(beatPath);
            }
        }
    }
    #endregion

    int countdown = 4;

    #region Spawn
    private void Countdown()
    {
        if (beatCount == beatDatas.Count)
        {
            visual.SetCountdownText("GO!");
            countdown = 4;
        }
        else if (beatCount > beatDatas.Count - countdown)
        {
            visual.EnableCountdown(true);
            countdown--;
            visual.SetCountdownText(countdown.ToString());
        }
        else
        {
            visual.EnableCountdown(false);
        }
    }
    #endregion

    #region Reset

    [Button]
    private void ResetMiniGame()
    {
        beatCount = 0;

        ClearNotes();

        visual.SetSpeakerImg(SpeakerStatus.Ready);

        mover.MoveLocal(speaker, speakerOriPos.localPosition, beatTime);
        mover.MoveLocal(amp, beatDatas[beatDatas.Count - 1].position, beatTime);
    }

    [Button]
    private void ClearNotes()
    {
        beatNotes.Clear();
        beatPaths.Clear();

        visual.ClearBeatNotes();
        visual.ClearBeatPath();
    }
    #endregion
}
