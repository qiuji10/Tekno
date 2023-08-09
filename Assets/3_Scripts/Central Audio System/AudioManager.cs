using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource SFX_Source, BGM_Source;


    [SerializeField] float bgmVolume;
    [SerializeField] float sfxVolume;

    void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;
    }

    public void SetVolume()
    {
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        SFX_Source.volume = sfxVolume;
    }

    public void SetBGMVolume()
    {
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        BGM_Source.volume = bgmVolume;
    }



    SoundFile GetSound(AudioData _audioType, string _name)
    {
        List<SoundFile> temp = new List<SoundFile>(_audioType.audioList);

        for (int i = 0; i < temp.Count; i++)
        {
            if (temp[i].name == _name)
            {
                return temp[i];
            }
        }

        return null;
    }

    SoundFile GetSound(AudioData _audioType, int index)
    {
        List<SoundFile> temp = new List<SoundFile>(_audioType.audioList);
        return temp[index];
    }

    SoundFile GetRandomSound(AudioData _audioType, string _name)
    {
        List<SoundPack> temp = new List<SoundPack>(_audioType.soundPacks);

        for (int i = 0; i < temp.Count; i++)
        {
            if (temp[i].name == _name)
            {
                return temp[i].audioList[Random.Range(0, temp[i].audioList.Count)];
            }
        }

        return null;
    }

    public void PlaySFX(AudioData _audioData, string _name)
    {
        SoundFile sound = GetSound(_audioData, _name);
        if (sound != null)
        {
            SFX_Source.volume = sound.volume;
            SFX_Source.PlayOneShot(sound.clip);
        }
    }

    public void PlaySFX(AudioData _audioData, int _index)
    {
        SoundFile sound = GetSound(_audioData, _index);
        if (sound != null)
        {
            SFX_Source.volume = sound.volume;
            SFX_Source.PlayOneShot(sound.clip);
        }
    }

    public void PlayRandomSFX(AudioData _audioData, string _name)
    {
        SoundFile sound = GetRandomSound(_audioData, _name);
        if (sound != null)
        {
            SFX_Source.volume = sound.volume;
            SFX_Source.PlayOneShot(sound.clip);
        }
    }

    public void PlayLoopingSFX(AudioData _audioData, string _name)
    {
        SoundFile sound = GetSound(_audioData, _name);
        if (sound != null)
        {
            SFX_Source.volume = sound.volume;
            SFX_Source.clip = sound.clip;
            SFX_Source.loop = true;

            SFX_Source.Play();
        }
    }

    public void PlayBGM(AudioData _audioData, string _name, bool loop = true)
    {
        SoundFile sound = GetSound(_audioData, _name);
        if (sound != null)
        {
            BGM_Source.volume = sound.volume;
            BGM_Source.clip = sound.clip;
            BGM_Source.loop = loop;

            BGM_Source.Play();
        }
    }

    public void StopBGM()
    {
        BGM_Source.Stop();
    }

    public void StopAllSound()
    {
        SFX_Source.Stop();
        BGM_Source.Stop();
    }
}
