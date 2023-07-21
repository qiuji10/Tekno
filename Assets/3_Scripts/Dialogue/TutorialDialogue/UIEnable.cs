using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class UIEnable : MonoBehaviour
{

    [SerializeField] private InputActionReference interactKey;
    [SerializeField] private bool disablePlayerControl = false;
    public GameObject healthCanvas, stanceManagerCanvas;
    public GameObject healthUIReference, stanceManagerUIReference;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject player;
    [SerializeField] private float triggerDelay;
    private void Start()
    {
        healthUIReference = GameObject.Find("Health Spectrum Canvas");
        stanceManagerUIReference = GameObject.Find("Stance Canvas");
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        interactKey.action.performed += Interact;
    }

    private void OnDisable()
    {
        interactKey.action.performed -= Interact;
    }


    private void Interact(InputAction.CallbackContext context)
    {
        StartCoroutine(waitFor());
    }

    public void EnableHealthUI()
    {
        if (healthUIReference != null)
        {
            healthUIReference.GetComponent<Canvas>().enabled = true;
            healthCanvas.GetComponent<Canvas>().enabled = true;
            playerController.DisableAction2();
            disablePlayerControl = true;

        }
        else
        {
            healthUIReference = null;
        }
    }

    public void StanceManagerUI()
    {
        if (stanceManagerUIReference != null)
        {
            stanceManagerUIReference.GetComponent<Canvas>().enabled = true;
            stanceManagerCanvas.GetComponent<Canvas>().enabled = true;
            playerController.DisableAction2();
            disablePlayerControl = true;
        }
        else
        {
            stanceManagerUIReference = null;
        }
    }


    public void DisableUICanvas()
    {
        healthCanvas.GetComponent<Canvas>().enabled = false;
        stanceManagerCanvas.GetComponent<Canvas>().enabled = false;
        if (disablePlayerControl == true)
        {
            playerController.EnableAction();
            disablePlayerControl = false;
        }
    }


    private IEnumerator waitFor()
    {
        yield return new WaitForSeconds(triggerDelay);
        if (healthCanvas.activeSelf)
        {
            DisableUICanvas();
        }

        if (stanceManagerCanvas.activeSelf)
        {
            DisableUICanvas();
        }
    }


}
