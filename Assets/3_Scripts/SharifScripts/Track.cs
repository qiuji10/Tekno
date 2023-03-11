using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Song")]
public class Track : ScriptableObject
{
    public Genre genre;
    public int bpm;
    public Koreography koreography;
    [Range(0f, 1f)] public float volume;
}
