using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetter : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCameraBase InUsedCam;
    [SerializeField] private bool isFollow;
    [SerializeField] private bool isLook;

    private void Start()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        if (isFollow) InUsedCam.Follow = player;
        if (isLook) InUsedCam.LookAt = player;
    }

    public void LoadCam()
    {
        InUsedCam.Priority = 11;
    }

    public void UnloadCam()
    {
        InUsedCam.Priority = 9;
    }

    public void SwitchCam(CinemachineVirtualCameraBase newVcam)
    {
        UnloadCam();
        InUsedCam = newVcam;
        LoadCam();
    }
}
