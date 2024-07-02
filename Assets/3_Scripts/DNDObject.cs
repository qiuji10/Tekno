using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNDObject : MonoBehaviour
{
    public GameSceneManager gameSceneManager;

    private void Update()
    {
        OnSceneChange();
    }

    public void OnSceneChange()
    {
        if (gameSceneManager.IsLoading == true)
        {
            DontDestroyOnLoad(this.gameObject);
            Debug.Log("Loading Next Scene, not destroying object");
        }
        else
        {
            gameSceneManager.IsLoading = false;
            Debug.Log("Next Scene Loaded");
        }
    }
}
