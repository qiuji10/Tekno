using Cinemachine;
using SonicBloom.Koreo.Players;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public enum Genre { House, Techno, Electronic, All }

public class StanceManager : MonoBehaviour
{
    public static Track curTrack;
    public static event Action<Track> OnStanceChangeStart;
    public static bool AllowPlayerSwitchStance;
    public static float changeStanceTime = 2.333f;

    [SerializeField] PlayableDirector director;
    private CinemachineDollyCart cart;

    [Header("Audio References")]
    [SerializeField] private SimpleMusicPlayer musicPlayer;
    [SerializeField] private AudioSource stanceAudio;
    [SerializeField] List<Track> tracks = new List<Track>();
    private int trackIndex;

    [Header("Ability References")]
    [SerializeField] private HookAbility hookAbility;
    [SerializeField] private TeleportAbility teleportAbility;

    [Header("Input Action References")]
    [SerializeField] private InputActionReference skipTrackAction;
    [SerializeField] private InputActionReference rewindTrackAction;

    private bool firstTimeIgnored;

    private void OnEnable()
    {
        skipTrackAction.action.performed += SkipTrack;
        rewindTrackAction.action.performed += RewindTrack;
    }

    private void OnDisable()
    {
        skipTrackAction.action.performed -= SkipTrack;
        rewindTrackAction.action.performed -= RewindTrack;
    }

    private void Awake()
    {
        AllowPlayerSwitchStance = true;
        cart = director.GetComponent<CinemachineDollyCart>();
        musicPlayer = GetComponent<SimpleMusicPlayer>();
    }

    private void Start()
    {
        trackIndex = 1;
        PlayTrack(trackIndex);
    }

    private void SkipTrack(InputAction.CallbackContext context)
    {
        if (!AllowPlayerSwitchStance) return;

        stanceAudio.time = 0;
        trackIndex++;
        trackIndex = trackIndex % tracks.Count;
        PlayTrack(trackIndex);
    }

    private void RewindTrack(InputAction.CallbackContext context)
    {
        if (!AllowPlayerSwitchStance) return;

        stanceAudio.time = 0;
        trackIndex = (trackIndex + tracks.Count - 1) % tracks.Count;
        PlayTrack(trackIndex);
    }

    private void PlayTrack(int index)
    {
        if (!AllowPlayerSwitchStance) return;

        stanceAudio.volume = tracks[index].volume;
        musicPlayer.LoadSong(tracks[index].koreography);
        curTrack = tracks[index];
        if (!firstTimeIgnored)
        {
            firstTimeIgnored = true;
        }
        else
        {
            if (director)
            {
                cart.m_Position = 0;
                director.time = 0;
                director.enabled = true;
                director.Play();
            }

            AllowPlayerSwitchStance = false;
            StartCoroutine(EnableInput(changeStanceTime));
            OnStanceChangeStart?.Invoke(tracks[index]);
        }
        //stanceAudio.Play();

        // here should ability switch
        switch (curTrack.genre)
        {
            case Genre.House:
                hookAbility.enabled = true;
                teleportAbility.enabled = false;
                break;
            case Genre.Techno:
                hookAbility.enabled = false;
                teleportAbility.enabled = false;
                break;
            case Genre.Electronic:
                hookAbility.enabled = false;
                teleportAbility.enabled = true;
                break;
        }
    }

    public void EnableSwitchStance()
    {
        AllowPlayerSwitchStance = true;
    }

    public void DisableSwitchStance()
    {
        AllowPlayerSwitchStance = false;
    }

    private IEnumerator EnableInput(float time)
    {
        PlayerController.allowedInput = false;
        yield return new WaitForSeconds(time);
        PlayerController.allowedInput = true;
        AllowPlayerSwitchStance = true;
        if (director) director.enabled = false;
    }

}
