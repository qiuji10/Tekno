using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameSceneManager : MonoBehaviour
{
    public static event Action<Scene> OnNewSceneLoaded;

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

        while (!loadingScene.isDone)
        {
            yield return null;
        }

        OnNewSceneLoaded?.Invoke(SceneManager.GetActiveScene());
    }
}
