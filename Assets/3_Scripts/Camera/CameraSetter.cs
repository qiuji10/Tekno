using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetter : MonoBehaviour
{
    [Header("Init Settings")]
    [SerializeField] private CinemachineVirtualCameraBase InUsedCam;
    [SerializeField] private bool isFollow;
    [SerializeField] private bool isLook;

    [Header("Blending Settings")]
    [SerializeField] private float blendTime = 1.0f;
    private CinemachineBrain brain;

    private void Awake()
    {
        brain = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineBrain>();
    }

    private void Start()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        if (isFollow) InUsedCam.Follow = player;
        if (isLook) InUsedCam.LookAt = player;
    }

    public void LoadCam()
    {
        brain.m_DefaultBlend.m_Time = blendTime;
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
