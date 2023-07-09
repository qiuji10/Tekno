using Cinemachine;
using SonicBloom.Koreo.Players;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UI;

public enum Genre { House, Techno, Electronic, All }

public class StanceManager : MonoBehaviour
{
    public static Track curTrack;
    public static event Action<Track> OnStanceChangeStart;
    public static bool AllowPlayerSwitchStance;
    public static float changeStanceTime = 2.333f;

    //[SerializeField] PlayableDirector director;
    //private CinemachineDollyCart cart;

    [Header("Audio References")]
    [SerializeField] private SimpleMusicPlayer musicPlayer;
    [SerializeField] private AudioSource stanceAudio;
    [SerializeField] List<Track> tracks = new List<Track>();
    private int trackIndex;

    [Header("UI Reference")]
    [SerializeField] TMPro.TMP_Text songNameText;
    [SerializeField] private Image r1_up_img;
    [SerializeField] private Image r1_down_img;
    [SerializeField] private Image r2_up_img;
    [SerializeField] private Image r2_down_img;

    [Header("Ability References")]
    [SerializeField] private HookAbility hookAbility;
    [SerializeField] private TeleportAbility teleportAbility;

    [Header("Input Action References")]
    [SerializeField] private InputActionReference skipTrackAction;
    [SerializeField] private InputActionReference rewindTrackAction;

    [Header("VFX")]
    [SerializeField] private GameObject[] shockwaveVFX;
    [SerializeField] private Transform spawnPos;
    public static float particleSystemTime = 1.2f;


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
        //cart = director.GetComponent<CinemachineDollyCart>();
        musicPlayer = GetComponent<SimpleMusicPlayer>();
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
            //if (director)
            //{
            //    cart.m_Position = 0;
            //    director.time = 0;
            //    director.enabled = true;
            //    director.Play();
            //}

            AllowPlayerSwitchStance = false;
            StartCoroutine(EnableInput(changeStanceTime));
            OnStanceChangeStart?.Invoke(tracks[index]);
        }
        //stanceAudio.Play();

        // here should ability switch
        switch (curTrack.genre)
        {
            case Genre.House:

                r1_up_img.color = r1_down_img.color = Color.green;
                r2_up_img.color = r2_down_img.color = Color.cyan;

                songNameText.color = Color.yellow;
                songNameText.text = "House - Aggression";
                hookAbility.enabled = true;
                teleportAbility.enabled = false;
                StartCoroutine(StanceShockwave(shockwaveVFX[0], spawnPos));

                break;
            case Genre.Techno:

                r1_up_img.color = r1_down_img.color = Color.yellow;
                r2_up_img.color = r2_down_img.color = Color.green;

                songNameText.color = Color.cyan;
                songNameText.text = "Techno - Treck No.1";
                hookAbility.enabled = false;
                teleportAbility.enabled = false;
                StartCoroutine(StanceShockwave(shockwaveVFX[1], spawnPos));

                break;
            case Genre.Electronic:

                r1_up_img.color = r1_down_img.color = Color.cyan;
                r2_up_img.color = r2_down_img.color = Color.yellow;

                songNameText.color = Color.green;
                songNameText.text = "Electro - Ready";
                hookAbility.enabled = false;
                teleportAbility.enabled = true;
                StartCoroutine(StanceShockwave(shockwaveVFX[2], spawnPos));

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
        //if (director) director.enabled = false;
    }

    private IEnumerator StanceShockwave(GameObject shockwaveSystem, Transform spawnPoint)
    {
        yield return new WaitForSeconds(1.65f);

        ParticleSystem particleSystem = Instantiate(shockwaveSystem, spawnPoint.position, spawnPoint.rotation).GetComponent<ParticleSystem>();
        particleSystem.Play();

        yield return new WaitForSeconds(particleSystemTime);

        particleSystem.Stop();
        Destroy(particleSystem.gameObject);
    }


}
