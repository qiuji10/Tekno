using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Menu/AudioSoundData")]
public class AudioData : ScriptableObject
{
    public List<SoundFile> audioList = new List<SoundFile>();

    public List<SoundPack> soundPacks = new List<SoundPack>();

}