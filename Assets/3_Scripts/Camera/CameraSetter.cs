using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetter : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCameraBase InUsedCam;

    public void LoadCam()
    {
        InUsedCam.Priority = 11;
    }

    public void UnloadCam()
    {
        InUsedCam.Priority = 9;
    }
}
