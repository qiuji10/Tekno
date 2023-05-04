using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorManager : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject gameplayTrigger;
    [SerializeField] private GameObject tutorialTrigger;
    [SerializeField] private GameObject lobbyTrigger;
    [SerializeField] private GameObject gameplayLoadTrigger;
    [SerializeField] private GameObject tutorialLoadTrigger;

    private PlayerData data;

    private void Awake()
    {
        data = FindObjectOfType<PlayerData>();
    }

    public void UnparentPlayer()
    {
        data.controller.transform.SetParent(null);
    }

    public void EvaluateDestination()
    {
        if (data.destination == ElevatorDestination.Gameplay)
        {
            gameplayLoadTrigger.SetActive(true);
            gameplayTrigger.SetActive(true);
            anim.SetTrigger("DownGameplay");
        }
        else if (data.destination == ElevatorDestination.Tutorial)
        {
            tutorialLoadTrigger.SetActive(true);
            tutorialTrigger.SetActive(true);
            anim.SetTrigger("DownTutorial");
        }
        else if (data.destination == ElevatorDestination.Lobby)
        {

        }
    }

    public void DisableAllTrigger()
    {
        gameplayTrigger.SetActive(false);
        tutorialTrigger.SetActive(false);
        //lobbyTrigger.SetActive(false);
    }
}
