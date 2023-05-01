using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElevatorDestination { Gameplay, Tutorial, Lobby }

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;
    public PlayerController controller;

    public ElevatorDestination destination;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
