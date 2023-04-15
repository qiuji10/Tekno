using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetter : MonoBehaviour
{
    [SerializeField] private Vector2 axisValue;
    [SerializeField] private Vector2 axisSpeed;

    private ThirdPerCam cam;

    private void Awake()
    {
        cam = Camera.main.GetComponent<ThirdPerCam>();
    }

    public void SetFreeLookCam()
    {
        cam.SetCamMode(CameraMode.FreeLook, axisValue, axisSpeed);
    }

    public void SetFixedCam()
    {
        cam.SetCamMode(CameraMode.Fixed, axisValue, axisSpeed);
    }
}
