using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;

public class GameSceneManager : MonoBehaviour
{
    public static event Action<Scene> OnNewSceneLoaded;

    public UnityEvent OnSceneInitialization;

    public List<GameObject> objectsToBeTransfer = new List<GameObject>();

    private void Awake()
    {
        OnSceneInitialization?.Invoke();
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(StartLoadingScene(sceneName, LoadSceneMode.Single));
    }

    public void LoadSceneAdditive(string sceneName)
    {
        StartCoroutine(StartLoadingScene(sceneName, LoadSceneMode.Additive));
    }

    private IEnumerator StartLoadingScene(string sceneName, LoadSceneMode mode)
    {
        AsyncOperation loadingScene = SceneManager.LoadSceneAsync(sceneName, mode);

        List<GameObject> objects = objectsToBeTransfer;

        while (!loadingScene.isDone)
        {
            yield return null;
        }

        //if (activateScene) SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        //Scene curScene = SceneManager.GetActiveScene();
        
        //GameSceneManager[] sceneManagers = FindObjectsOfType<GameSceneManager>();
        //Debug.Log(curScene.name);
        //foreach (GameSceneManager sceneManager in sceneManagers)
        //{
        //    if (sceneManager.gameObject.scene != curScene)
        //    {
        //        Debug.Log("heyhey");
        //        sceneManager.objectsToBeTransfer = objects;
        //    }
        //}

        //foreach (GameObject obj in objects)
        //{
        //    SceneManager.MoveGameObjectToScene(obj, curScene);
        //}

        OnNewSceneLoaded?.Invoke(SceneManager.GetActiveScene());
    }

    public void UnloadScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }
}
