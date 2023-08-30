using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioManager))]
public class UIAudioManager : MonoBehaviour
{
    [SerializeField] AudioData audioData;

    public void playSFX(string name)
    {
        AudioManager.instance.PlaySFX(audioData, name);
    }
}
