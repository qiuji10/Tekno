using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Song")]
public class Track : ScriptableObject
{
    //public string name;
    public Genre genre;
    public AudioClip clip;

    public AudioClip Clip
    {
        get { return clip; }
        set { clip = value; }
    }

    public Genre Genre
    {
        get { return genre; }
        set { genre = value; }
    }
}
