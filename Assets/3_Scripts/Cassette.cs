using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum Genre
{
    House,
    Elecktronic,
    DeepDown,
}

public class Cassette : MonoBehaviour
{
    private Genre type;

    public List<SongObject> Songs = new List<SongObject>();
    private SongObject songObject;
    private Button addSongButton;
    public GameObject songListContainer;
    public AudioSource audioSource;
    public GameObject SongMenu;

    public int songIndex = 0;
    public int rewindTime = 10;
    public bool menuOpen = false;
    //public Material material;
    private void Awake()
    {
        //Debug.Log(songObject.genre);
        //Debug.Log(songObject.name);

    }

    public void Update()
    {

        //display song list 
        if (Input.GetKeyDown(KeyCode.Q))
        {
            foreach (SongObject song in Songs)
            {
                Debug.Log(song);
            }

            if (Songs.Count == 0)
            {
                Debug.Log("List is empty");
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            PlaySong(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SkipSong();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Rewind();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (!menuOpen)
            {
                EnableMenu(true);
            }
            else
            {
                EnableMenu(false);
            }
        }


        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Songs.RemoveAt(0);
        }




        //UpdateCassetteVisual();
    }



    /*
    void UpdateCassetteVisual()
    {
        if(type == Genre.House)
        {
            material.color = Color.blue;
        }

        if (type == Genre.Elecktronic)
        {
            material.color = Color.green;
        }

        if (type == Genre.DeepDown)
        {
            material.color = Color.yellow;
        }
    }
    */

    public void EnableMenu(bool open)
    {
        if (open)
        {
            menuOpen = true;
            SongMenu.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            menuOpen = false;
            SongMenu.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    //add songs to the list 
    public void AddSong(SongObject song)
    {
        Songs.Add(song);
    }

    //remove songs at the first index
    void RemoveSong(int index)
    {
        Songs.RemoveAt(index);

        // Update the UI to reflect the change in the song list
    }

    //skip current song and play next songs in the list
    void SkipSong()
    {
        audioSource.time = 0;
        songIndex++;

        if (songIndex >= Songs.Count)
        {
            songIndex = 0;
        }

        PlaySong(songIndex);
    }

    //rewind the song X amount of time (can be set in the editor)
    //if the song is rewind more than the current lenght of the song, replay the previous song at the extra time
    void Rewind()
    {
        int numOfSongs = Songs.Count;
        if (audioSource.time < rewindTime)
        {
            float extraRewindTime = 0;
            //audioSource.time = 0;
            extraRewindTime = rewindTime - audioSource.time;
            if (songIndex > 0)
            {
                songIndex--;
                PlaySong(songIndex);
                audioSource.time = Songs[songIndex].clip.length - extraRewindTime;
            }
            else
            {
                Replay();
            }

            //Songs[songIndex].clip.length
        }
        else
        {
            audioSource.time -= rewindTime;
        }

    }

    void Replay()
    {
        audioSource.time = 0;   
    }

    void PlaySong(int index)
    {
        audioSource.clip = Songs[index].clip;
        audioSource.Play();
        //audioSource.volume;
        //audioSource.pitch;
    }
}
