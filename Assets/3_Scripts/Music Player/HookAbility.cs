using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HookAbility : MonoBehaviour
{
    [Header("Hook Settings")]
    [SerializeField] private float throwForwardBurst = 50f;
    [SerializeField] private float throwBehindBurst = -50f;
    [SerializeField] private float hookTime = 7.4f;
    [SerializeField] private float successRatio = 0.8f;
    [SerializeField] private float dotProduct;
    [SerializeField] private float launchAmount;
    

    [SerializeField] private Vector3 detectOffset;
    [SerializeField] private Vector3 handPoint;
    [SerializeField] private Vector3 grabOffset;

    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private SensorDetection hookSensor;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private InputActionReference hookAction;
    [SerializeField] private Slider hookSlider;
    [SerializeField] private Image hookViableVisual;

    private PlayerController _playerController;
    private Rigidbody _rb;
    private Pendulum pendulum;
    private float convertLerpRatio;

    public bool isHooking;
    private float timer;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _rb = GetComponent<Rigidbody>();
        //hookViableVisual.fillAmount = 1 - successRatio;
    }

    private void OnEnable()
    {
        hookAction.action.started += Hook;
        hookAction.action.canceled += Hook;
    }

    private void OnDisable()
    {
        hookAction.action.started -= Hook;
        hookAction.action.canceled -= Hook;
    }

    private void Update()
    {
        if (isHooking)
        {
            HookMovement();
        }
    }

    private void Hook(InputAction.CallbackContext context)
    {
        
        if (isHooking)
        {
            //if (timer / hookTime < successRatio)
            //    DisableHook(false);
            //else
            //    DisableHook(true);

            if(context.canceled)
            {
                DisableHook(false);
            }
        }
        else
        {
            //if (context.canceled) return;

            pendulum = hookSensor.GetNearestObject<Pendulum>();

            Debug.Log(pendulum);

            throwBehindBurst = 0;
            throwForwardBurst = 0;

            if (pendulum != null)
            {
                Vector3 direction = orientation.transform.position - pendulum.transform.position;

                Transform pendulumForward = pendulum.transform;

                dotProduct = Vector3.Dot(direction.normalized, pendulumForward.position);

                if (dotProduct >= 0f)
                {
                    pendulum.startSineRatio = -Mathf.Abs(pendulum.startSineRatio);
                    
                }
                else
                {
                    pendulum.startSineRatio = Mathf.Abs(pendulum.startSineRatio);
                }
                hookSlider.gameObject.SetActive(true);
                Transform playerTR = _playerController.transform;

                playerTR.SetParent(pendulum.transform);
                playerTR.localPosition = grabOffset;
                playerTR.localEulerAngles = Vector3.zero;
                playerTR.GetChild(0).localEulerAngles = Vector3.zero;

                _rb.isKinematic = true;
                _rb.constraints = RigidbodyConstraints.None;

                //_cam.enabled = false;
                _playerController.Anim.SetTrigger("StartSwing");
                //_playerController.enabled = false;
                PlayerController.allowedInput = false;
                //hookSlider.value = timer = 0;
                convertLerpRatio = pendulum.lerpRatio;
                //hookSlider.value = convertLerpRatio;
                //hookSlider.maxValue = hookTime;

                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, pendulum.transform.position);

                pendulum.isUsingHook = true;
                isHooking = true;
            }
        }
    }

    private void HookMovement()
    {
        handPoint = lineRenderer.transform.position;
        float ratio = pendulum.lerpRatio;
        hookSlider.value = ratio;

        lineRenderer.SetPosition(1, handPoint);

        if (pendulum.isForward)
        {
            throwBehindBurst = 0;
            throwForwardBurst += launchAmount * Time.deltaTime;
        }
        else
        {
            throwForwardBurst = 0;
            throwBehindBurst -= launchAmount * Time.deltaTime;
        }

    }

    private void DisableHook(bool enableForce)
    {
        hookSlider.gameObject.SetActive(false);
        isHooking = false;
        timer = 0;

        _rb.isKinematic = false;

        if (!enableForce)
        {
            if(pendulum.isForward)
            {
                _rb.AddForce(pendulum.transform.forward * throwForwardBurst, ForceMode.Impulse);
            }
            else
            {
                _rb.AddForce(pendulum.transform.forward * throwBehindBurst, ForceMode.Impulse);
            }
            
        }

        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        _playerController.transform.SetParent(null);

        //_cam.enabled = true;
        PlayerController.allowedInput = true;
        //_playerController.enabled = true;
        _playerController.transform.eulerAngles = Vector3.zero;
        _playerController.Anim.SetTrigger("EndSwing");

        lineRenderer.positionCount = 0;

        pendulum.isUsingHook = false;
        pendulum.ResetPendulum(pendulum.startSineRatio);
        pendulum = null;
    }
}