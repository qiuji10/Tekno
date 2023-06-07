using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SettingsSystem : MonoBehaviour
{
    [Header("Audio Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Audio Mixer Group")]
    [SerializeField] private AudioMixerGroup masterMixer;
    [SerializeField] private AudioMixerGroup bgmMixer;
    [SerializeField] private AudioMixerGroup sfxMixer;

    [Header("Audio Test")]
    [SerializeField] private AudioSource audioTester;
    [SerializeField] private AudioClip bgmClipTest;
    [SerializeField] private AudioClip sfxClipTest;

    [Header("Audio Visuals")]
    [SerializeField] private Sprite lowVolSprite;
    [SerializeField] private Sprite medVolSprite;
    [SerializeField] private Sprite highVolSprite;

    [Header("Audio Visuals")]
    [SerializeField] private Sprite lowBriSprite;
    [SerializeField] private Sprite medBriSprite;
    [SerializeField] private Sprite highBriSprite;

    [Header("BrightnessSlider")]
    [SerializeField] private Slider brightnessSlider;

    private float maxBrightness = 1f; // The maximum brightness value, adjust as needed

    private const string masterVolumeKey = "MasterVolume";
    private const string bgmVolumeKey = "BgmVolume";
    private const string sfxVolumeKey = "SfxVolume";
    private const string brightnessKey = "Brightness";

    private void Start()
    {
        // Set initial values for sliders
        if (PlayerPrefs.HasKey(masterVolumeKey))
            masterSlider.value = PlayerPrefs.GetFloat(masterVolumeKey, GetMasterVolume());
        else
            masterSlider.value = 0.5f;

        if (PlayerPrefs.HasKey(bgmVolumeKey))
            bgmSlider.value = PlayerPrefs.GetFloat(bgmVolumeKey, GetBgmVolume());
        else
            masterSlider.value = 0.5f;

        if (PlayerPrefs.HasKey(sfxVolumeKey))
            sfxSlider.value = PlayerPrefs.GetFloat(sfxVolumeKey, GetSfxVolume());
        else
            masterSlider.value = 0.5f;

        if (PlayerPrefs.HasKey(brightnessKey))
            brightnessSlider.value = PlayerPrefs.GetFloat(brightnessKey, GetBrightness());
        else
            masterSlider.value = GetBrightness();

        SetMasterVolume(masterSlider.value);
        SetBgmVolume(bgmSlider.value);
        SetSfxVolume(sfxSlider.value);
        SetBrightness(brightnessSlider.value);

        gameObject.SetActive(false);
    }

    public void OnMasterVolumeUpdate(float volume)
    {
        SetMasterVolume(volume);
        PlayerPrefs.SetFloat(masterVolumeKey, volume);
    }

    public void OnBgmVolumeUpdate(float volume)
    {
        SetBgmVolume(volume);
        PlayerPrefs.SetFloat(bgmVolumeKey, volume);
    }

    public void OnSfxVolumeUpdate(float volume)
    {
        SetSfxVolume(volume);
        PlayerPrefs.SetFloat(sfxVolumeKey, volume);
    }

    public void OnBrightVolumeUpdate(float brightness)
    {
        SetBrightness(brightness);
        PlayerPrefs.SetFloat(brightnessKey, brightness);
    }

    private void SetMasterVolume(float volume)
    {
        float value = volume == 0 ? -80f : Mathf.Log10(volume) * 20f; // Convert volume value to logarithmic scale
        masterMixer.audioMixer.SetFloat("MasterVolume", value);
    }

    private float GetMasterVolume()
    {
        float value;
        masterMixer.audioMixer.GetFloat("MasterVolume", out value);
        return Mathf.Pow(10f, value / 20f); // Convert volume value from logarithmic scale to linear scale
    }

    private void SetBgmVolume(float volume)
    {
        float value = volume == 0 ? -80f : Mathf.Log10(volume) * 20f;
        bgmMixer.audioMixer.SetFloat("BgmVolume", value);
    }

    private float GetBgmVolume()
    {
        float value;
        bgmMixer.audioMixer.GetFloat("BgmVolume", out value);
        return Mathf.Pow(10f, value / 20f);
    }

    private void SetSfxVolume(float volume)
    {
        float value = volume == 0 ? -80f : Mathf.Log10(volume) * 20f;
        sfxMixer.audioMixer.SetFloat("SfxVolume", value);
    }

    private float GetSfxVolume()
    {
        float value;
        sfxMixer.audioMixer.GetFloat("SfxVolume", out value);
        return Mathf.Pow(10f, value / 20f);
    }

    private void SetBrightness(float brightness)
    {
        Screen.brightness = brightness;
    }

    private float GetBrightness()
    {
        return Screen.brightness;
    }
}
