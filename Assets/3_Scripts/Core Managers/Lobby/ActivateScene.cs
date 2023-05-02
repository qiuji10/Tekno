using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivateScene : MonoBehaviour
{
    public string sceneName;

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
    }
}
