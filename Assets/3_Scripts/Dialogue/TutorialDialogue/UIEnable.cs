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
    [SerializeField] private float triggerDelay,enableDelay = 0.9f;

    private bool healthCanvasEnabledRecently = false;
    private bool stanceManagerCanvasEnabledRecently = false;

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
        if (healthCanvas.activeSelf)
        {
            if (healthCanvasEnabledRecently)
            {
                DisableCanvas();
            }
        }

        if (stanceManagerCanvas.activeSelf)
        {
            if (stanceManagerCanvasEnabledRecently)
            {
                DisableCanvas();
            }
        }
    }

    private IEnumerator EnableHealthCanvasWithDelay()
    {
        yield return new WaitForSeconds(enableDelay);
        healthUIReference.GetComponent<Canvas>().enabled = true;
        healthCanvas.GetComponent<Canvas>().enabled = true;

        if (disablePlayerControl == false)
        {
            playerController.DisableAction();
            disablePlayerControl = true; 
            yield return new WaitForSeconds(triggerDelay);
            healthCanvasEnabledRecently = true;
        }
       
       
    }

    private IEnumerator EnableStanceManagerCanvasWithDelay()
    {
        yield return new WaitForSeconds(enableDelay);
        stanceManagerUIReference.GetComponent<Canvas>().enabled = true;
        stanceManagerCanvas.GetComponent<Canvas>().enabled = true;
        if (disablePlayerControl == false)
        {
            playerController.DisableAction();
            disablePlayerControl = true;
            yield return new WaitForSeconds(triggerDelay);
            stanceManagerCanvasEnabledRecently = true;
        }
       
    }

    public void EnableHealthUI()
    {
        if (healthUIReference != null)
        {
            StartCoroutine(EnableHealthCanvasWithDelay());
        }
        else
        {
            healthUIReference = null;
        }
    }

    public void EnableStanceManagerUI()
    {
        if (stanceManagerUIReference != null)
        {
            StartCoroutine(EnableStanceManagerCanvasWithDelay());
        }
        else
        {
            stanceManagerUIReference = null;
        }
    }

    public void DisableCanvas()
    {
        healthCanvas.GetComponent<Canvas>().enabled = false;
        stanceManagerCanvas.GetComponent<Canvas>().enabled = false;
        if (disablePlayerControl)
        {
            playerController.EnableAction();
            disablePlayerControl = false;
        }
    }

  
}
