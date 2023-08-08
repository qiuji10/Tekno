using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsKeyCollector : MonoBehaviour
{
    public static PlayerPrefsKeyCollector Instance;

    public List<string> dialogueKeys;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    [Button]
    public void ClearPlayerPrefs()
    {
        foreach (string key in dialogueKeys)
        {
            PlayerPrefs.DeleteKey(key);
        }

        PlayerPrefs.Save();
    }
}
