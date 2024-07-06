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
        if (GameSceneManager.IsLoading == true)
        {
            DontDestroyOnLoad(this.gameObject);
            Debug.Log("Current Scene Manager before changing scenes: " + GameSceneManager.IsLoading);
        }
        else
        {
            GameSceneManager.IsLoading = false;
            Debug.Log("Current Scene Manager after changing scenes: " + GameSceneManager.IsLoading);
        }
    }
}
