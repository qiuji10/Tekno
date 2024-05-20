using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

public partial class Minigame : MonoBehaviour
{
    public enum State { None, Spawn, Play, Fail }

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

    //public float beatTime => TempoManager.GetTimeToBeatCount(1);
    public float beatTime => (140f / 60f) / 60f;

    private void Awake()
    {
        visual.OpenPanel(false);
    }

    private void OnEnable()
    {
        TempoManager.OnBeat += TempoManager_OnBeat;
    }

    private void OnDisable()
    {
        TempoManager.OnBeat -= TempoManager_OnBeat;
    }

    public void StartGame(List<BeatSequence> data)
    {
        this.data = data;

        visual.OpenPanel(true);

        CheckLevel();
    }

    private void CheckLevel()
    {
        if (level < data.Count)
        {
            beatDatas = data[level].beatSettings;
            level++;

            input = new Input(inputReference);
            input.Init(this, beatDatas, speaker);
            input.OnFailure += OnFail;

            ResetMiniGame();
            SpawnNotes(level);

            state = State.Spawn;
        }
        else
        {
            // End
        }
    }

    private void InitSetup()
    {
        input = new Input(inputReference);
        input.Init(this, beatDatas, speaker);
        input.OnFailure += OnFail;

        ResetMiniGame();
        SpawnNotes(level);

        state = State.Spawn;
    }

    private void OnFail(string msg)
    {
        state = State.Fail;

        beatCount = 0;

        visual.SetPromptText(msg);

        //InitSetup();
    }

    private void TempoManager_OnBeat()
    {
        if (state == State.Spawn)
        {
            beatNotes[beatCount].SetActive(true);

            if (beatCount < beatPaths.Count)
                beatPaths[beatCount].SetPreviewValue(0, 1, beatTime);

            beatCount++;

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
                visual.EnableCountdown(false);

            if (beatCount < beatNotes.Count && beatCount >= 0)
                beatNotes[beatCount].gameObject.SetActive(true);

            if (beatCount < beatPaths.Count && beatCount >= 0)
                beatPaths[beatCount].SetPathValue(0, 1, beatTime);

            beatCount++;

            if (beatCount < beatDatas.Count)
            {
                mover.MoveLocal(speaker, beatDatas[beatCount].position, beatTime);
                input.StartCheck(beatDatas[beatCount].position, beatDatas[beatCount].key);
            }
            else
            {
                CheckLevel();
            }
        }
        else if (state == State.Fail)
        {
            beatCount++;

            visual.SetPromptText("");

            if (beatCount >= 1)
                InitSetup();
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

    #region Spawn
    private void Countdown()
    {
        int diff = beatNotes.Count - beatCount;

        if (diff <= 4 && beatCount < beatNotes.Count)
        {
            int countdown = diff - 1;

            if (countdown == 0)
                visual.SetCountdownText("GO!");
            else
                visual.SetCountdownText($"{countdown}");
        }
    }
    #endregion

    #region Reset
    [Button]
    private void ResetMiniGame()
    {
        beatCount = 0;

        ClearNotes();

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
