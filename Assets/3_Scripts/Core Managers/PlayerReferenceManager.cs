using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReferenceManager : MonoBehaviour
{
    private static PlayerReferenceManager instance;
    public static PlayerReferenceManager Instance
    {
        get { return instance; }
    }

    public GameObject[] playerReferenceObjects;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}