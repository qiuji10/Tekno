using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IntroScreen : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCameraBase vcam;
    public Canvas canvas;

    public UnityEvent OnAnyKeyDown;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("Intro") && PlayerPrefs.GetInt("Intro") == 1)
        {
            OnAnyKeyDown?.Invoke();
            canvas.enabled = false;
            vcam.Priority = 0;
            gameObject.SetActive(false);
        }
        else
        {
            PlayerPrefs.SetInt("Intro", 1);
        }
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            OnAnyKeyDown?.Invoke();
            canvas.enabled = false;
            vcam.Priority = 0;
            gameObject.SetActive(false);
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("Intro", 0);
    }

    [Button]
    private void ClearIntroPlayerPref()
    {
        PlayerPrefs.SetInt("Intro", 0);
    }
}
