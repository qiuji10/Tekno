using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ElevatorDestination { Tutorial, Lobby, Gameplay}

public class ElevatorSystem : MonoBehaviour
{
    [SerializeField] private float totalTimeToDestination = 7f;

    [Header("Option Refrences")]
    [SerializeField] private Button goToCityButton;
    [SerializeField] private Button tutorialButton;

    [Header("Trigger References")]
    [SerializeField] private Collider goToDestinationTrigger;
    [SerializeField] private Collider resetTrigger;

    [Header("Player")]
    [SerializeField] private Transform player;

    private bool isMovingUp => destination < currentElevatorPlacement;
    private int openDoor, closeDoor;
    private ElevatorDestination destination;
    private ElevatorDestination currentElevatorPlacement;
    private Animator elevatorAnim;

    public Dictionary<ElevatorDestination, Vector3> destinationData; // the vector3 is holding local position

    private void Awake()
    {
        destinationData = new Dictionary<ElevatorDestination, Vector3>
        {
            { ElevatorDestination.Tutorial, new Vector3(0, 40, 0) },
            { ElevatorDestination.Lobby, new Vector3(0, 0, 0) },
            { ElevatorDestination.Gameplay, new Vector3(0, -60, 0) }
        };

        currentElevatorPlacement = ElevatorDestination.Lobby;

        player = GameObject.FindGameObjectWithTag("Player").transform;

        elevatorAnim = GetComponent<Animator>();

        openDoor = Animator.StringToHash("OpenDoor");
        closeDoor = Animator.StringToHash("CloseDoor");

        CloseDoor();
    }

    private void Start()
    {
        MainMenu.NewGameSelected += UpdateOption;
    }

    private void OnDestroy()
    {
        MainMenu.NewGameSelected -= UpdateOption;
    }

    public void SetDestination(string destination)
    {
        this.destination = (ElevatorDestination)Enum.Parse(typeof(ElevatorDestination), destination);
    }

    public void CloseDoor()
    {
        elevatorAnim.SetTrigger(closeDoor);
    }

    public void OpenDoor()
    {
        elevatorAnim.SetTrigger(openDoor);
    }

    public void UpdateOption()
    {
        goToCityButton.interactable = false;
        goToCityButton.GetComponentInChildren<TMP_Text>().color = Color.black;
    }

    public void SelectButton()
    {
        if (goToCityButton.IsInteractable())
        {
            EventSystem.current.SetSelectedGameObject(goToCityButton.gameObject);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(tutorialButton.gameObject);
        }
    }

    [Button]
    public void GoToDestination()
    {
        StartCoroutine(GoToDestination_Coroutine());
    }

    private IEnumerator GoToDestination_Coroutine()
    {
        if (currentElevatorPlacement == destination)
            yield break;

        resetTrigger.enabled = true;
        goToDestinationTrigger.enabled = false;

        CloseDoor();

        Vector3 currentPosition = transform.localPosition;
        Vector3 destinationPosition = destinationData[destination];

        float totalTime = totalTimeToDestination;
        float timer = 0;

        yield return new WaitForSeconds(1.2f);

        while (timer <= totalTime)
        {
            timer += Time.fixedDeltaTime;
            float ratio = timer / totalTime;

            transform.localPosition = Vector3.Lerp(currentPosition, destinationPosition, ratio);
            yield return new WaitForFixedUpdate();
        }

        currentElevatorPlacement = destination;
        transform.localPosition = destinationPosition;
        OpenDoor();
    }

    public void OnPlayerEnterElevator()
    {
        if (player && !isMovingUp)
        {
            player.parent = transform;
        }
    }

    public void OnPlayerExitElevator()
    {
        if (player)
        {
            player.parent = null;
        }
    }
}
