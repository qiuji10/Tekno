using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HookAbility : MonoBehaviour
{
    [Header("Hook Settings")]
    [SerializeField] private float throwForwardBurst = 50f;
    [SerializeField] private float hookTime = 7.4f;
    [SerializeField] private float successRatio = 0.8f;

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
    //private ThirdPerCam _cam;
    private Rigidbody _rb;
    private Pendulum pendulum;

    public bool isHooking;
    private float timer;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _rb = GetComponent<Rigidbody>();
        //_cam = Camera.main.GetComponent<ThirdPerCam>();
        hookViableVisual.fillAmount = 1 - successRatio;
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
            if (timer / hookTime < successRatio)
                DisableHook(false);
            else
                DisableHook(true);
        }
        else
        {
            if (context.canceled) return;

            pendulum = hookSensor.GetNearestObject<Pendulum>();

            if (pendulum != null) 
            {
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
                _playerController.enabled = false;

                hookSlider.value = timer = 0;
                hookSlider.maxValue = hookTime;

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

        timer += Time.deltaTime;
        hookSlider.value = timer;

        lineRenderer.SetPosition(1, handPoint);

        if (timer > hookTime)
            DisableHook(false);
    }

    private void DisableHook(bool enableForce)
    {
        hookSlider.gameObject.SetActive(false);
        isHooking = false;
        timer = 0;

        _rb.isKinematic = false;

        if (enableForce)
        {
            _rb.AddForce(pendulum.transform.forward * throwForwardBurst, ForceMode.Impulse);
        }

        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        _playerController.transform.SetParent(null);
        
        //_cam.enabled = true;
        _playerController.enabled = true;
        _playerController.transform.eulerAngles = Vector3.zero;
        _playerController.Anim.SetTrigger("EndSwing");

        lineRenderer.positionCount = 0;

        pendulum.isUsingHook = false;
        pendulum.ResetPendulum();
        pendulum = null;
    }
}
