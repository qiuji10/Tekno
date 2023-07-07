using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public static bool canPause = true;
    public static bool isPause = false;

    [SerializeField] private InputActionReference optionAction;

    [SerializeField] private Canvas canvas;
    [SerializeField] private Volume vol;
    [SerializeField] private GameObject objToBeSelected;
   
    private DepthOfField dofComponent;


    private void OnEnable()
    {
        optionAction.action.performed += Action_performed;

        //volumeProfile.TryGetSubclassOf(typeof(DepthOfField), out dofComponent);
        if (vol) vol.profile.TryGet(out dofComponent);
    }

    private void OnDisable()
    {
        optionAction.action.performed -= Action_performed;
        isPause = false;
        canPause = true;
    }

    private void Action_performed(InputAction.CallbackContext obj)
    {
        if (isPause)
        {
            ResumeGame();
        }
        else if (canPause && !isPause)
        {
            PauseGame();
        }
        
    }

    [Button]
    public void PauseGame()
    {
        if (!canPause) return;

        canvas.enabled = true;
        if (dofComponent) dofComponent.focalLength.value = 50f;
        isPause = true;
        EventSystem.current.SetSelectedGameObject(objToBeSelected);
        Time.timeScale = 0;
    }

    [Button]
    public void ResumeGame()
    {
        canvas.enabled = false;
        if (dofComponent) dofComponent.focalLength.value = 1.0f;
        isPause = false;
        EventSystem.current.SetSelectedGameObject(null);
        Time.timeScale = 1;
    }

    public void BackToLobby()
    {
        Time.timeScale = 1;
        StartCoroutine(BackToLobby_Coroutine());
    }

    private IEnumerator BackToLobby_Coroutine()
    {
        MaterialModifier modifier = FindObjectOfType<MaterialModifier>();
        modifier.ResetMaterial();

        NonDestructible[] nonDestructibles = FindObjectsOfType<NonDestructible>();

        foreach (NonDestructible item in nonDestructibles)
        {
            Destroy(item.gameObject);
            yield return null;
        }

        FindObjectOfType<GameSceneManager>().LoadScene("1_Lobby");
    }
}
