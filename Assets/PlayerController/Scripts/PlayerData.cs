using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    //public static PlayerData Instance;
    public GameObject player;
    public PlayerController controller;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        controller = player.GetComponent<PlayerController>();
    }

    public void EnablePlayerController()
    {
        if (controller == null)
        {
            controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        controller.enabled = true;
    }

    public void DisablePlayerController()
    {
        controller.enabled = false;
    }
}
