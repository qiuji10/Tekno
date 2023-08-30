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

    #region Dump
    //private IEnumerator StartLoadingScene(string sceneName, LoadSceneMode mode)
    //{
    //    if (IsSceneLoadedAdditively(sceneName)) yield break;

    //    AsyncOperation loadingScene = SceneManager.LoadSceneAsync(sceneName, mode);

    //    while (!loadingScene.isDone)
    //    {
    //        yield return null;
    //    }

    //    Scene newScene = SceneManager.GetSceneByName(sceneName);

    //    if (newScene.name != "1_Lobby")
    //    {
    //        GameObject[] playerObjs = PlayerReferenceManager.Instance.playerReferenceObjects;

    //        for (int i = 0; i < playerObjs.Length; i++)
    //        {
    //            if (playerObjs[i].transform.parent != null)
    //            {
    //                continue;
    //            }

    //            SceneManager.MoveGameObjectToScene(playerObjs[i], newScene);
    //            yield return null;
    //        }
    //    }

    //    if (newScene.name == "Q1 Map Test v2_old")
    //    {
    //        SceneManager.SetActiveScene(newScene);
    //    }

    //    //Scene curScene = SceneManager.GetActiveScene();

    //    //GameSceneManager[] sceneManagers = FindObjectsOfType<GameSceneManager>();
    //    //Debug.Log(curScene.name);
    //    //foreach (GameSceneManager sceneManager in sceneManagers)
    //    //{
    //    //    if (sceneManager.gameObject.scene != curScene)
    //    //    {
    //    //        Debug.Log("heyhey");
    //    //        sceneManager.objectsToBeTransfer = objects;
    //    //    }
    //    //}

    //    //foreach (GameObject obj in objects)
    //    //{
    //    //    SceneManager.MoveGameObjectToScene(obj, curScene);
    //    //}

    //    OnNewSceneLoaded?.Invoke(SceneManager.GetActiveScene());
    //} 
    #endregion

    private IEnumerator StartLoadingScene(string sceneName, LoadSceneMode mode)
    {
        if (SceneWasLoaded(sceneName)) yield break;

        AsyncOperation loadingScene = SceneManager.LoadSceneAsync(sceneName, mode);

        while (!loadingScene.isDone)
        {
            yield return null;
        }

        Scene newScene = SceneManager.GetSceneByName(sceneName);

        SceneManager.SetActiveScene(newScene);

        OnNewSceneLoaded?.Invoke(newScene);
    }

    public void UnloadScene(string sceneName)
    {

        StartCoroutine(StartUnloadScene(sceneName));
        
    }

    #region Dump
    //private IEnumerator StartUnloadScene(string sceneName)
    //{
    //    Scene newScene = SceneManager.GetActiveScene();

    //    if (newScene.name != "1_Lobby")
    //    {
    //        GameObject[] playerObjs = PlayerReferenceManager.Instance.playerReferenceObjects;

    //        for (int i = 0; i < playerObjs.Length; i++)
    //        {
    //            if (playerObjs[i].transform.parent != null)
    //            {
    //                playerObjs[i].transform.SetParent(null);
    //            }

    //            SceneManager.MoveGameObjectToScene(playerObjs[i], newScene);
    //            yield return null;
    //        }
    //    }

    //    SceneManager.UnloadSceneAsync(sceneName);
    //} 
    #endregion

    private IEnumerator StartUnloadScene(string sceneName)
    {
        int sceneCount = SceneManager.sceneCount;

        for (int i = 0; i < sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            if (scene.name == sceneName)
            {
                SceneManager.UnloadSceneAsync(sceneName);
            }
        }

        yield return null;
    }

    public static bool SceneWasLoaded(string sceneName)
    {
        int sceneCount = SceneManager.sceneCount;

        for (int i = 0; i < sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            if (scene.name == sceneName)
            {
                return true;
            }
        }

        return false;
    }
}
