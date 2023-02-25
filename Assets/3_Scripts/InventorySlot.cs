using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public GameObject playList;
    public GameObject storedSongList;
    public GameObject songPlaceHolder;

    private int songCount;
    public Cassette cassette;
    public SongManager songManager;

    public RectTransform playListRect;

    //public DragObject dragObject;

    public void Update()
    {
        //creates a new list of songs every time the key is pressed (temporary mix system)
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {

            if (gameObject == playList)
            {
                cassette.Songs.Clear();
                int numOfSong = 0;

                //goes through all the children from the playList gameObject to find SongManager componet
                for (int i = 0; i < playList.transform.childCount; i++)
                {
                    songManager = playList.transform.GetChild(numOfSong).GetComponent<SongManager>();

                    //if the componet is there it will add it to the Songs List in cassette script
                    if (songManager != null)
                    {
                        cassette.AddSong(songManager.songData);
                        numOfSong++;
                    }

                }

                Debug.Log(numOfSong);
            }



        }

    }
    public void OnDrop(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        GameObject dropped = eventData.pointerDrag;
        DragObject dragObject = dropped.GetComponent<DragObject>();
        dragObject.parentAfterDrag = transform;
    }
}
