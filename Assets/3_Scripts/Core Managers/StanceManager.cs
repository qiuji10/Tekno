using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Genre { House, Techno, Electronic }

public class StanceManager : MonoBehaviour
{
    public static Genre curStance;
    public static event Action<Track> OnStanceChange;

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
        musicPlayer = GetComponent<SimpleMusicPlayer>();
    }

    private void Start()
    {
        PlayTrack(1);
    }

    private void SkipTrack(InputAction.CallbackContext context)
    {
        stanceAudio.time = 0;
        trackIndex++;
        trackIndex = trackIndex % tracks.Count;
        PlayTrack(trackIndex);
    }

    private void RewindTrack(InputAction.CallbackContext context)
    {
        stanceAudio.time = 0;
        trackIndex = (trackIndex + tracks.Count - 1) % tracks.Count;
        PlayTrack(trackIndex);
    }

    private void PlayTrack(int index)
    {
        stanceAudio.volume = tracks[index].volume;
        musicPlayer.LoadSong(tracks[index].koreography);
        curStance = tracks[index].genre;
        OnStanceChange?.Invoke(tracks[index]);
        //stanceAudio.Play();

        // here should ability switch
        switch (curStance)
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
}
