using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetter : MonoBehaviour
{
    [SerializeField] private float xValue;
    [SerializeField] private float yValue;

    private ThirdPerCam cam;

    private void Awake()
    {
        cam = Camera.main.GetComponent<ThirdPerCam>();
    }

    public void SetFreeLookCam()
    {
        cam.SetCamMode(CameraMode.FreeLook, xValue, yValue);
    }

    public void SetFixedCam()
    {
        cam.SetCamMode(CameraMode.Fixed, xValue, yValue);
    }
}
