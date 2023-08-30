using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public GameObject continueButton;
    public GameObject newGameButton;

    public static event Action NewGameSelected;

    private void Awake()
    {
        ValidateStatus(false);
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
