using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElevatorDestination { Gameplay, Tutorial, Lobby }

public class PlayerData : MonoBehaviour
{
    //public static PlayerData Instance;
    public PlayerController controller;

    public ElevatorDestination destination;

    //private void Awake()
    //{
    //    if (Instance == null)
    //    {
    //        Instance = this;
    //    }
    //    else
    //    {
    //        Destroy(this.gameObject);
    //        return;
    //    }

    //    DontDestroyOnLoad(gameObject);
    //}

    private void Start()
    {
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public void EnablePlayerController()
    {
        controller.enabled = true;
    }

    public void DisablePlayerController()
    {
        controller.enabled = false;
    }
}
