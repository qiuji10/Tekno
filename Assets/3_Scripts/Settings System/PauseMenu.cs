using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.EventSystems;
using NodeCanvas.BehaviourTrees;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool canPause = true;
    public static bool isPause = false;

    [SerializeField] private InputActionReference optionAction;

    [SerializeField] private Canvas canvas;
    [SerializeField] private Volume vol;
    [SerializeField] private GameObject objToBeSelected;
   
    private DepthOfField dofComponent;
    private PlayerController playerController;
    private Coroutine ensureUIRoutine;
    [SerializeField, ReadOnly] private GameObject lastSelectedUI;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

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

    // IMPORTANT TODO:
    // Reassign back the set selected gameobject to the settings button, in scene

    private void Action_performed(InputAction.CallbackContext obj)
    {
        if (DialogueManager.IsRunning)
            return;

        if (isPause)
        {
            ResumeGame();
        }
        else if (canPause && !isPause)
        {
            PauseGame();
        }
        
    }

    private void TryStopRoutine()
    {
        if (ensureUIRoutine != null)
            StopCoroutine(ensureUIRoutine);
    }

    private IEnumerator EnsureLastSelectedObjectRoutine()
    {
        while (true)
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                lastSelectedUI = EventSystem.current.currentSelectedGameObject;
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(lastSelectedUI);
            }

            yield return null;
        }
    }

    [Button]
    public void PauseGame()
    {
        if (!canPause) return;

        canvas.enabled = true;
        if (dofComponent) dofComponent.focalLength.value = 50f;
        isPause = true;
        playerController.DisableAction();
        EventSystem.current.SetSelectedGameObject(objToBeSelected);
        Time.timeScale = 0;

        TryStopRoutine();

        ensureUIRoutine = StartCoroutine(EnsureLastSelectedObjectRoutine());
    }

    [Button]
    public void ResumeGame()
    {
        canvas.enabled = false;
        if (dofComponent) dofComponent.focalLength.value = 1.0f;
        isPause = false;
        playerController.EnableAction();
        EventSystem.current.SetSelectedGameObject(null);
        Time.timeScale = 1;

        TryStopRoutine();
    }

    public void BackToLobby()
    {
        Time.timeScale = 1;
        StartCoroutine(BackToLobby_Coroutine());
    }

    private IEnumerator BackToLobby_Coroutine()
    {
        Destroy(FindObjectOfType<PlayerController>().gameObject);

        BehaviourTreeOwner[] enemies = FindObjectsOfType<BehaviourTreeOwner>();

        foreach (BehaviourTreeOwner item in enemies)
        {
            item.gameObject.SetActive(false);
            yield return null;
        }

        MaterialModifier modifier = FindObjectOfType<MaterialModifier>();
        modifier.ResetMaterial();

        if (FadeCanvas.Instance)
            FadeCanvas.Instance.FadeOut();
        yield return new WaitForSeconds(1f);

        string curScene = SceneManager.GetActiveScene().name;

        SceneManager.UnloadSceneAsync(curScene);
        SceneManager.LoadScene("Base Scene (Elevator)");
    }
}
