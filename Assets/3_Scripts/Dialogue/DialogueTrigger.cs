using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using NaughtyAttributes;

public enum SaveState { None, PlayerPrefs }

public class DialogueTrigger : MonoBehaviour
{
    [Header("Basic Settings")]
    [SerializeField, ShowIf("IS_PLAYER_PREFS")] private string dialogueName;
    [SerializeField] private SaveState saveState = SaveState.None;
    [SerializeField] private bool markAsPlayerPrefs;
    [SerializeField] private bool triggerOnce;
    [SerializeField] private bool disablePlayerControl = true;

    [Header("Interaction Settigs")]
    [SerializeField, ShowIf("IS_STAY")] private GameObject prompt;
    [SerializeField, ShowIf("IS_STAY")] private InputActionReference interactKey;
    [SerializeField] private EventInvokeType eventType = EventInvokeType.Stay;
    [SerializeField] private Genre genre = Genre.All;

    [Header("Events")]
    [SerializeField] private UnityEvent OnInteract;
    [SerializeField] private UnityEvent OnInteractEnd;

    private bool inRange, triggerDisable;
    private Camera cam;
    private PlayerController player;

    private void Start()
    {
        cam = Camera.main;
    }

    private void OnEnable()
    {
        if (interactKey) interactKey.action.performed += Interact;
        DialogueManager.OnDialogueEnd += DialogueManager_OnDialogueEnd;
    }

    private void OnDisable()
    {
        if (interactKey) interactKey.action.performed -= Interact;
        DialogueManager.OnDialogueEnd -= DialogueManager_OnDialogueEnd;
    }

    private bool CheckPlayerPrefs()
    {
        return PlayerPrefs.HasKey(dialogueName) && PlayerPrefs.GetInt(dialogueName) == 1;
    }

    private void Interact(InputAction.CallbackContext context)
    {
        if (CheckPlayerPrefs())
            return;

        if (StanceManager.curTrack.genre != genre && genre != Genre.All) return;

        if (!triggerDisable && eventType == EventInvokeType.Stay && inRange)
        {
            if (triggerOnce)
            {
                triggerDisable = true;
                prompt.SetActive(false);
            }

            OnInteract?.Invoke();

            if (saveState == SaveState.PlayerPrefs)
            {
                PlayerPrefs.SetInt(dialogueName, 1);
                PlayerPrefs.Save();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CheckPlayerPrefs())
            return;

        if (StanceManager.curTrack.genre != genre && genre != Genre.All) return;

        if (!triggerDisable && !inRange && other.CompareTag("Player"))
        {
            if (prompt != null) prompt.SetActive(true);

            if (eventType == EventInvokeType.Enter)
            {
                if (triggerOnce)
                {
                    triggerDisable = true;
                    if (prompt != null) prompt.SetActive(false);
                }
                
                OnInteract?.Invoke();

                if (saveState == SaveState.PlayerPrefs)
                {
                    PlayerPrefs.SetInt(dialogueName, 1);
                    PlayerPrefs.Save();
                }

                //if (player == null)
                ////   other.TryGetComponent(out player);
                //player = other.GetComponent<PlayerController>();
                
                
                //if (player && disablePlayerControl)
                //{
                //    player.DisableAction();
                //}
            }

            inRange = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!triggerDisable && inRange && prompt != null)
            {
                prompt.transform.rotation = Quaternion.LookRotation(prompt.transform.position - cam.transform.position);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (prompt != null) prompt.SetActive(false);
            inRange = false;
        }
    }

    private void DialogueManager_OnDialogueEnd()
    {
        if (inRange)
        {
            OnInteractEnd?.Invoke();

            //if (disablePlayerControl)
            //{
            //    player.EnableAction();
            //}
        }
    }

    private bool IS_ENTER() { return eventType == EventInvokeType.Enter; }
    private bool IS_STAY() { return eventType == EventInvokeType.Stay; }
    private bool IS_PLAYER_PREFS() { return saveState == SaveState.PlayerPrefs; }
}
