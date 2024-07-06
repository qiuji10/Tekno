using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject continueButton;
    public GameObject newGameButton;

    [SerializeField, ReadOnly] private GameObject lastSelectedUI;

    private Coroutine ensureUIRoutine;

    public static event Action NewGameSelected;

    private void Awake()
    {
        ValidateStatus(false);
        continueButton.GetComponent<Button>().onClick.AddListener(TryStopRoutine);
        newGameButton.GetComponent<Button>().onClick.AddListener(TryStopRoutine);
    }

    public void StartNewGame()
    {
        PlayerPrefsKeyCollector.Instance.ClearPlayerPrefs();
        PlayerPrefs.SetInt("has_save", 1);
        PlayerPrefs.Save();

        ValidateStatus(true);

        NewGameSelected?.Invoke();
    }

    public void ValidateStatus(bool selectButton)
    {
        if (PlayerPrefs.HasKey("has_save") && PlayerPrefs.GetInt("has_save") == 1)
        {
            continueButton.SetActive(true);

            if (selectButton)
                EventSystem.current.SetSelectedGameObject(continueButton);
        }
        else
        {
            continueButton.SetActive(false);

            if (selectButton)
                EventSystem.current.SetSelectedGameObject(newGameButton);
        }

        TryStopRoutine();

        ensureUIRoutine = StartCoroutine(EnsureLastSelectedObjectRoutine());
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

    public void Quit()
    {
        PlayerPrefs.SetInt("has_save", 0);
        PlayerPrefs.SetInt("intro_cutscene", 0);
        PlayerPrefs.DeleteKey("intro_cutscene");
        PlayerPrefs.Save();
        Application.Quit();
    }
}
