using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using System;

public class SlidingRailAbility : MonoBehaviour
{
    [SerializeField] private CinemachinePathBase m_Path;
    [SerializeField] private float slidingSpeed = 30;
    
    [Header("Input Action")]
    [SerializeField] private InputActionReference jumpAction;

    private float m_Speed;
    private float m_Position;
    private bool isGrindingRail;

    private Rigidbody rb;
    private Collider _collider;
    private CinemachinePathBase.PositionUnits m_PositionUnits = CinemachinePathBase.PositionUnits.Distance;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        jumpAction.action.started += OnJump;
    }

    private void OnDisable()
    {
        jumpAction.action.started -= OnJump;
    }

    void FixedUpdate()
    {
        if (isGrindingRail)
        {
            if (m_Position >= m_Path.PathLength)
            {
                rb.isKinematic = false;
                rb.AddForce(transform.GetChild(0).forward * 15);
                StartCoroutine(DelayAction(EndSlidingPipe, 0.2f));
            }
            else
            {
                SetCartPosition(m_Position + m_Speed * Time.deltaTime);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isGrindingRail && collision.transform.CompareTag("Rail"))
        {
            m_Path = collision.transform.GetComponentInParent<CinemachineSmoothPath>();
            float d = (float)collision.transform.GetSiblingIndex() / (float)collision.transform.parent.childCount;

            //Vector3 tempPos = m_Path.EvaluatePositionAtUnit(0, m_PositionUnits);
            //d = Vector3.Distance(transform.position, tempPos);

            //for (float i = 0.01f; i < 1; i += 0.01f)
            //{
            //    Vector3 pointPos = m_Path.EvaluatePositionAtUnit(m_Path.StandardizeUnit(i, m_PositionUnits), m_PositionUnits);
            //    float cacheDist = Vector3.Distance(transform.position, pointPos);

            //    if (cacheDist <= d)
            //        d = cacheDist;
            //    else
            //        break;
            //}
            //d = d / m_Path.PathLength;
            if (d > 0.95) return;

            
            m_Position = d * m_Path.PathLength;

            // Calculate the angle between the two directions
            //float angle = Quaternion.Angle(transform.GetChild(0).rotation, collision.transform.rotation);
            //bool sameDir = angle <= 90 || angle >= 360 - 90;
            //m_Speed = sameDir ? slidingSpeed : -slidingSpeed;
            //transform.eulerAngles = sameDir? Vector3.zero : new Vector3(0, 0, -1);

            m_Speed = slidingSpeed;
            transform.eulerAngles = Vector3.zero;
            transform.GetChild(0).localEulerAngles = Vector3.zero;
            StartSlidingPipe();
        }
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (isGrindingRail)
        {
            EndSlidingPipe();

            rb.AddForce(Vector3.up * 10, ForceMode.Impulse);
        }
    }

    public void StartSlidingPipe()
    {
        isGrindingRail = true;
        PlayerController.allowedInput = false;
        ThirdPerCam.allowedRotation = false;
        rb.isKinematic = true;
        _collider.enabled = false;
    }

    public void EndSlidingPipe()
    {
        if (isGrindingRail)
        {
            PlayerController.allowedInput = true;
            ThirdPerCam.allowedRotation = true;
            rb.isKinematic = false;
            transform.parent = null;
            m_Path = null;
            m_Speed = 0;
            isGrindingRail = false;
            transform.GetChild(0).localEulerAngles = new Vector3(0, transform.GetChild(0).localEulerAngles.y, 0);

            StartCoroutine(EnableBackCollider());
        }
    }

    private void SetCartPosition(float distanceAlongPath)
    {
        if (m_Path != null)
        {
            m_Position = m_Path.StandardizeUnit(distanceAlongPath, m_PositionUnits);
            transform.position = m_Path.EvaluatePositionAtUnit(m_Position, m_PositionUnits);
            transform.GetChild(0).rotation = m_Path.EvaluateOrientationAtUnit(m_Position, m_PositionUnits);
        }
    }

    private IEnumerator EnableBackCollider()
    {
        yield return new WaitForSeconds(0.05f);
        _collider.enabled = true;
    }

    private IEnumerator DelayAction(Action callback, float delay)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }
}
