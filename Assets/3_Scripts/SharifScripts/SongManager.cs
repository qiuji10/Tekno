using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongManager : MonoBehaviour
{
    public Track songData;

    private void Start()
    {
        Genre genre = songData.genre;
        AudioClip audioClip = songData.clip;
    }
}
