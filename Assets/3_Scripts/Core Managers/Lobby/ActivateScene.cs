using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivateScene : MonoBehaviour
{
    public string sceneName;

    private void Start()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
    }
}
