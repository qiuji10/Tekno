using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum CameraMode { FreeLook, Fixed }

public class ThirdPerCam : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerObj;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private float rotationSpeed = 7f;

    [Header("CM Reference")]
    [SerializeField] private CinemachineFreeLook cm;
    private CinemachineInputProvider cmInput;

    public static bool allowedRotation;

    private void Awake()
    {
        allowedRotation = true;
        cmInput = cm.GetComponent<CinemachineInputProvider>();
    }

    private void Update()
    {
        if (!allowedRotation) return;

        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        Vector2 move = moveAction.action.ReadValue<Vector2>();

        Vector3 inputDir = orientation.forward * move.y + orientation.right * move.x;

        if (inputDir != Vector3.zero)
            playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
    }

    public void SetCamMode(CameraMode mode, float x = 0, float y = 0.5f, float time = 0.5f)
    {
        switch (mode)
        {
            case CameraMode.FreeLook:
                cmInput.enabled = true;
                break;

            case CameraMode.Fixed:

                cmInput.enabled = false;

                float startY = cm.m_YAxis.Value;
                float startX = cm.m_XAxis.Value;

                LeanTween.value(cm.gameObject, startY, y, time).setEaseInOutCirc().setOnUpdate((float value) => {
                    cm.m_YAxis.Value = value;
                });

                LeanTween.value(cm.gameObject, startX, x, time).setEaseInOutCirc().setOnUpdate((float value) => {
                    cm.m_XAxis.Value = value;
                });

                break;
        }

    }
}
