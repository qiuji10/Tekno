using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_Gyro : MonoBehaviour
{
    [SerializeField] float space = 50;
    [SerializeField] float sensitivity = 100;

    private Vector2 minPos;
    private Vector2 maxPos;
    private Gamepad controller = null;
    private RectTransform imageRect;

    void Start()
    {
        if (Gamepad.current != null)
            controller = DS4.getConroller();
        

        imageRect = GetComponent<RectTransform>();

        minPos.x = imageRect.anchoredPosition.x - space;
        minPos.y = imageRect.anchoredPosition.y - space;

        maxPos.x = imageRect.anchoredPosition.x + space;
        maxPos.y = imageRect.anchoredPosition.y + space;
    }

    void Update()
    {
        

        if (controller != null)
        {
            Vector2 newPos = imageRect.anchoredPosition;
            Quaternion rotation = DS4.getRotation();
            newPos.y -= rotation.x * sensitivity;
            //newPos.x -= rotation.z * sensitivity;
            //newPos.x = Mathf.Clamp(newPos.x, minPos.x, maxPos.x);
            newPos.y = Mathf.Clamp(newPos.y, minPos.y, maxPos.y);
            imageRect.anchoredPosition = newPos;
        }
    }
}
