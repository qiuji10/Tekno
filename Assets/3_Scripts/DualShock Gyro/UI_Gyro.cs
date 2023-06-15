using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Gyro : MonoBehaviour
{
    [SerializeField] float space = 50;
    [SerializeField] float sensitivity = 100;

    private Vector2 minPos;
    private Vector2 maxPos;
    private RectTransform imageRect;
    private Gyroscope gyro;

    void Start()
    {
        Input.gyro.updateInterval = 0.01f;
        gyro = Input.gyro;
        gyro.enabled = true;

        imageRect = transform as RectTransform;

        minPos.x = imageRect.anchoredPosition.x - space;
        minPos.y = imageRect.anchoredPosition.y - space;

        maxPos.x = imageRect.anchoredPosition.x + space;
        maxPos.y = imageRect.anchoredPosition.y + space;
    }

    void Update()
    {
        Vector2 newPos = imageRect.anchoredPosition;
        Quaternion rotation = gyro.attitude;
        newPos.y -= rotation.x * sensitivity;
        newPos.x -= rotation.z * sensitivity;
        newPos.x = Mathf.Clamp(newPos.x, minPos.x, maxPos.x);
        newPos.y = Mathf.Clamp(newPos.y, minPos.y, maxPos.y);
        imageRect.anchoredPosition = newPos;
    }
}
