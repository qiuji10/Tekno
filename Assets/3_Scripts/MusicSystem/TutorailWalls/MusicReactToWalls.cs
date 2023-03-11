using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicReactToWalls : MonoBehaviour
{

    [SerializeField] private Material material;
    [SerializeField] private Material materialStatic;

    private void OnEnable()
    {
        StanceManager.OnStanceChange += StanceManager_OnStanceChange;
    }

    private void StanceManager_OnStanceChange(Track obj)
    {
        float speed = 0.0f;
        Color gridColor = Color.clear;

        switch (obj.genre)
        {
            case Genre.House:
                speed = 1.0f;
                gridColor = new Color(3.92452836f, 3.58071637f, 0, 0);
                break;

            case Genre.Techno:
                speed = 2.2f;
                gridColor = new Color(0, 0.205526888f, 3.92452836f, 0);
                break;

            case Genre.Electronic:
                speed = 3.0f;
                gridColor = new Color(0.564901888f, 5.23955345f, 0.173004225f, 0);
                break;

            default:
                speed = 1.0f;
                gridColor = Color.white;
                break;
        }

        // Set the new values for the shader graph parameters
        material.SetFloat("_speed", speed);
        material.SetColor("_GridColor", gridColor);
        materialStatic.SetColor("_GridColor", gridColor);
    } 

    private void OnDisable()
    {
        StanceManager.OnStanceChange -= StanceManager_OnStanceChange;
    }
}
