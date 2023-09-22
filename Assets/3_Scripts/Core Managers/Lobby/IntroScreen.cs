using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IntroScreen : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCameraBase vcam;
    public Canvas canvas;
    public PlayerData playerData;

    public UnityEvent OnAnyKeyDown;

    private bool playerIn;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("intro_cutscene") && PlayerPrefs.GetInt("intro_cutscene") == 1)
            return;

        Collider[] overlapColliders = Physics.OverlapSphere(transform.position, 5);

        for (int i = 0; i < overlapColliders.Length; i++)
        {
            if (overlapColliders[i].CompareTag("Player"))
            {
                playerIn = true;
            }
        }

        DisableSelf();
    }

    private void Start()
    {
        FadeCanvas.Instance.FadeIn();
    }

    private void Update()
    {
        if (Input.anyKeyDown && playerIn)
        {
            EndIntro();
        }
    }

    private void EndIntro()
    {
        PlayerPrefs.SetInt("intro_cutscene", 1);
        PlayerPrefs.Save();

        OnAnyKeyDown?.Invoke();
        playerData.controller.EnableWithRestriction();
        canvas.enabled = false;
        vcam.Priority = 0;
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playerIn)
        {
            playerIn = true;
            playerData.controller.DisableAction();
        }
    }

    private void DisableSelf()
    {
        if (!playerIn)
        {
            canvas.enabled = false;
            gameObject.SetActive(false);
        }
    }
}
