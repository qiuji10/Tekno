using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class UIEnable : MonoBehaviour
{
    public GameObject healthUIReference, stanceManagerUIReference;
    [SerializeField] private InputActionReference DisableCanvas;
    public GameObject healthCanvas,stanceManagerCanvas;

    private void Start()
    {
        healthUIReference = GameObject.Find("Health Spectrum Canvas");
        stanceManagerUIReference = GameObject.Find("Stance Canvas");
       
    }

   public void EnableHealthUI()
    {
        if (healthUIReference != null)
        {
            healthUIReference.GetComponent<Canvas>().enabled = true;
        }
        else
        {
            healthUIReference = null;
        }
    }

   public  void StanceManagerUI()
    {
        if (stanceManagerUIReference != null)
        {
            stanceManagerUIReference.GetComponent<Canvas>().enabled = true;
        }
        else
        {
            stanceManagerUIReference = null;
        }
    }

   
   public void DisableUICanvas()
    {
            healthCanvas.SetActive(false);
            stanceManagerCanvas.SetActive(false); 
    }
}
